using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Audio;
using System.IO;

public static class SetupBugsFinalPolishUtility
{
    private static readonly string prefabDir   = "Assets/Prefabs";
    private static readonly string modelDir    = "Assets/Art/Models";
    private static readonly string texDir      = "Assets/Art/Textures";
    private static readonly string animDir     = "Assets/Art/Animations";
    private static readonly string audioDir    = "Assets/Audio/Bugs";

    [MenuItem("Tools/Final Bug Polish")]
    public static void FinalBugPolish()
    {
        // Variants and boss
        string[] variants = { "TankBug", "FastBug", "HealthBug" };
        foreach (var name in variants)
            ProcessVariant(name);
        ProcessBoss("BossBug");

        // Balancing
        var sm = Object.FindObjectOfType<SpawnManager>();
        if (sm != null)
        {
            sm.tankProbability = 0.15f;
            sm.fastProbability = 0.12f;
            sm.healProbability = 0.08f;
            sm.spawnInterval   = 0.8f;
            sm.bossWave        = 6;
            EditorUtility.SetDirty(sm);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[FinalBugPolish] Completed art, animation, audio, and balance setup.");
    }

    private static void ProcessVariant(string variant)
    {
        string prefabPath = Path.Combine(prefabDir, variant + ".prefab");
        if (!File.Exists(prefabPath))
        {
            Debug.LogWarning($"Prefab not found: {prefabPath}");
            return;
        }
        var contents = PrefabUtility.LoadPrefabContents(prefabPath);

        // 1. Replace primitive with model
        string modelPath = Path.Combine(modelDir, variant + ".fbx");
        var modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (modelAsset != null)
        {
            // destroy existing mesh children
            foreach (var mf in contents.GetComponentsInChildren<MeshFilter>(true))
                EditorUtility.DestroyImmediate(mf.gameObject);

            // instantiate model
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset, contents.transform);
            instance.name = variant + "Model";
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
        }

        // 2. Assign textures to material
        string matPath = Path.Combine(prefabDir, variant + "-Material.mat");
        var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat != null)
        {
            // Albedo, Normal, Emission textures
            var albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(texDir, variant + "_Albedo.png"));
            var normal = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(texDir, variant + "_Normal.png"));
            var emissn = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(texDir, variant + "_Emission.png"));

            if (albedo) mat.SetTexture("_MainTex", albedo);
            if (normal)
            {
                mat.EnableKeyword("_NORMALMAP");
                mat.SetTexture("_BumpMap", normal);
            }
            if (emissn)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white);
                mat.SetTexture("_EmissionMap", emissn);
            }
        }

        // 3. Hook up animations
        var animator = contents.GetComponent<Animator>();
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Bug.controller");
        if (animator != null && controller != null)
        {
            animator.runtimeAnimatorController = controller;
            // Add variant-specific clips
            var walk = AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(animDir, variant + "_Walk.anim"));
            var run  = AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(animDir, variant + "_Run.anim"));
            var die  = AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(animDir, variant + "_Die.anim"));
            var sm = controller.layers[0].stateMachine;
            if (walk != null) sm.AddState(variant + "_Walk").motion = walk;
            if (run != null)  sm.AddState(variant + "_Run").motion  = run;
            if (die != null)  sm.AddState(variant + "_Die").motion  = die;
        }

        // 4. Assign audio clips
        var tb = contents.GetComponent<TankBug>();
        var fb = contents.GetComponent<FastBug>();
        var hb = contents.GetComponent<HealthBug>();
        if (tb != null)
        {
            tb.hitSound    = LoadAudio(variant + "_Hit.wav");
            tb.deathSound  = LoadAudio(variant + "_Death.wav");
        }
        if (fb != null)
        {
            fb.spawnSound  = LoadAudio(variant + "_Spawn.wav");
        }
        if (hb != null)
        {
            hb.healSound   = LoadAudio(variant + "_Heal.wav");
        }

        PrefabUtility.SaveAsPrefabAsset(contents, prefabPath);
        PrefabUtility.UnloadPrefabContents(contents);
    }

    private static void ProcessBoss(string boss)
    {
        string prefabPath = Path.Combine(prefabDir, boss + ".prefab");
        if (!File.Exists(prefabPath)) return;
        var contents = PrefabUtility.LoadPrefabContents(prefabPath);

        // Replace mesh
        string modelPath = Path.Combine(modelDir, boss + ".fbx");
        var modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (modelAsset != null)
        {
            foreach (var mf in contents.GetComponentsInChildren<MeshFilter>(true))
                EditorUtility.DestroyImmediate(mf.gameObject);
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset, contents.transform);
            inst.name = boss + "Model";
            inst.transform.localPosition = Vector3.zero;
            inst.transform.localRotation = Quaternion.identity;
            inst.transform.localScale = Vector3.one * 2f;
        }

        // Audio: boss stinger and hit
        var bb = contents.GetComponent<BossBug>();
        if (bb != null)
        {
            bb.hitEffect   = null;
            bb.deathEffect = null;
            // play boss stinger on spawn
            var audioSource = contents.GetComponent<AudioSource>() ?? contents.AddComponent<AudioSource>();
            audioSource.clip = LoadAudio("Boss_Stinger.wav");
            audioSource.playOnAwake = true;
        }

        PrefabUtility.SaveAsPrefabAsset(contents, prefabPath);
        PrefabUtility.UnloadPrefabContents(contents);
    }

    private static AudioClip LoadAudio(string fileName)
    {
        string path = Path.Combine(audioDir, fileName);
        return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
    }
}
