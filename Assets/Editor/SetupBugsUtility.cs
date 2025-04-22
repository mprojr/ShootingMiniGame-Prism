using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public static class SetupBugsVisualsUtility
{
    private static readonly string prefabDir = "Assets/Prefabs";
    private static readonly string materialDir = "Assets/Materials";
    private static readonly string animDir = "Assets/Animations";
    private static readonly string particleDir = "Assets/Prefabs/Effects";
    private static readonly string uiDir = "Assets/UI";

    [MenuItem("Tools/Setup Bug Visuals")]
    public static void SetupVisuals()
    {
        // Ensure directories exist
        Directory.CreateDirectory(animDir);
        Directory.CreateDirectory(particleDir);
        Directory.CreateDirectory(uiDir);

        // 1. Create AnimatorController
        var controller = CreateAnimatorController();

        // 2. Create particle effect prefabs
        CreateParticlePrefab("Explosion", Color.red);
        CreateParticlePrefab("HealOrb", Color.green);
        CreateParticlePrefab("SpawnPuff", Color.cyan);

        // 3. Create BossHealthBar prefab
        CreateBossHealthBarPrefab();

        // 4. Update each bug prefab
        var variants = new[] {
            new { name = "TankBug", prim = PrimitiveType.Sphere, mat = "TankBug-Material.mat", scale=1.2f },
            new { name = "FastBug", prim = PrimitiveType.Capsule, mat = "FastBug-Material.mat", scale=1.0f },
            new { name = "HealthBug", prim = PrimitiveType.Cylinder, mat = "HealthBug-Material.mat", scale=1.0f },
            new { name = "BossBug", prim = PrimitiveType.Cube, mat = "BossBug-Material.mat", scale=2.0f }
        };

        foreach (var v in variants)
        {
            string path = Path.Combine(prefabDir, v.name + ".prefab");
            if (!File.Exists(path)) continue;

            // Load contents
            var go = PrefabUtility.LoadPrefabContents(path);

            // Clear existing mesh children
            var children = go.GetComponentsInChildren<MeshRenderer>().Select(r=>r.gameObject).ToList();
            foreach (var c in children) Object.DestroyImmediate(c);

            // Create primitive
            var primGO = GameObject.CreatePrimitive(v.prim);
            primGO.name = v.name + "Mesh";
            var rend = primGO.GetComponent<Renderer>();
            var mat = AssetDatabase.LoadAssetAtPath<Material>(Path.Combine(materialDir, v.mat));
            if (rend != null && mat != null)
                rend.sharedMaterial = mat;
            primGO.transform.SetParent(go.transform);
            primGO.transform.localPosition = Vector3.zero;
            primGO.transform.localRotation = Quaternion.identity;
            primGO.transform.localScale = Vector3.one * v.scale;

            // Add Animator
            var animator = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;

            // Wire particle refs if scripts exist
            if (v.name == "TankBug" && go.GetComponent<TankBug>() is TankBug tb)
            {
                tb.hitEffect = LoadParticle("Explosion");
                tb.deathEffect = LoadParticle("Explosion");
            }
            if (v.name == "HealthBug" && go.GetComponent<HealthBug>() is HealthBug hb)
            {
                hb.healEffect = LoadParticle("HealOrb");
            }
            if (v.name == "FastBug" && go.GetComponent<FastBug>() is FastBug fb)
            {
                fb.spawnEffect = LoadParticle("SpawnPuff");
            }
            if (v.name == "BossBug" && go.GetComponent<BossBug>() is BossBug boss)
            {
                boss.hitEffect = LoadParticle("Explosion");
                boss.deathEffect = LoadParticle("Explosion");
                // assign health bar prefab
                boss.healthBarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(uiDir, "BossHealthBar.prefab"));
            }

            // Save
            PrefabUtility.SaveAsPrefabAsset(go, path);
            PrefabUtility.UnloadPrefabContents(go);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("[SetupBugsVisualsUtility] Visuals setup complete.");
    }

    private static AnimatorController CreateAnimatorController()
    {
        string acPath = Path.Combine(animDir, "Bug.controller");
        var ac = AssetDatabase.LoadAssetAtPath<AnimatorController>(acPath);
        if (ac != null) return ac;

        ac = AnimatorController.CreateAnimatorControllerAtPath(acPath);
        var clip = new AnimationClip { name = "Pulse", legacy = false };
        // Scale pulse on x,y,z
        var curve = AnimationCurve.EaseInOut(0, 1, 1, 1.1f);
        clip.SetCurve("", typeof(Transform), "m_LocalScale.x", curve);
        clip.SetCurve("", typeof(Transform), "m_LocalScale.y", curve);
        clip.SetCurve("", typeof(Transform), "m_LocalScale.z", curve);
        AssetDatabase.CreateAsset(clip, Path.Combine(animDir, "Bug_Pulse.anim"));

        var rootState = ac.layers[0].stateMachine.AddState("Pulse");
        rootState.motion = clip;
        ac.layers[0].stateMachine.defaultState = rootState;

        return ac;
    }

    private static void CreateParticlePrefab(string name, Color col)
    {
        string path = Path.Combine(particleDir, name + ".prefab");
        if (File.Exists(path)) return;

        var go = new GameObject(name);
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = col;
        main.startSize = 0.5f;
        main.startLifetime = 0.5f;
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;

        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
    }

    private static ParticleSystem LoadParticle(string name)
    {
        return AssetDatabase.LoadAssetAtPath<ParticleSystem>(Path.Combine(particleDir, name + ".prefab"));
    }

    private static void CreateBossHealthBarPrefab()
    {
        string path = Path.Combine(uiDir, "BossHealthBar.prefab");
        if (File.Exists(path)) return;

        var go = new GameObject("BossHealthBar");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        var sliderGO = new GameObject("HealthSlider");
        sliderGO.transform.SetParent(go.transform);
        var slider = sliderGO.AddComponent<Slider>();

        // Background
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(sliderGO.transform);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = Color.red;
        slider.targetGraphic = bgImg;

        // Fill Area & Fill
        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGO.transform);
        var fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(fillArea.transform);
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = Color.green;
        slider.fillRect = fillImg.rectTransform;

        // Set some default rect transforms
        sliderGO.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 20);
        bgGO.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        bgGO.GetComponent<RectTransform>().anchorMax = Vector2.one;
        bgImg.rectTransform.sizeDelta = Vector2.zero;

        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
    }
}
