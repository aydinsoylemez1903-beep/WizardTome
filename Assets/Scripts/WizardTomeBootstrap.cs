using UnityEngine;

// WizardTome's placeholder world is generated at runtime to keep the initial repository
// compact and immediately editable without external meshes or imported assets.
public sealed class WizardTomeBootstrap : MonoBehaviour
{
    private static bool spawned;
    private static Material grass, stone, bark, leaf, violet, gold, dark, crystal;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        spawned = false;
        var root = new GameObject("WizardTome Runtime Bootstrap");
        root.AddComponent<WizardTomeBootstrap>();
        DontDestroyOnLoad(root);
    }

    private void Start()
    {
        if (spawned) return;
        spawned = true;

        grass = MakeMaterial(new Color(.20f, .43f, .28f));
        stone = MakeMaterial(new Color(.33f, .31f, .46f));
        bark = MakeMaterial(new Color(.30f, .20f, .23f));
        leaf = MakeMaterial(new Color(.13f, .37f, .35f));
        violet = MakeMaterial(new Color(.56f, .28f, .92f));
        gold = MakeMaterial(new Color(.98f, .68f, .24f));
        dark = MakeMaterial(new Color(.16f, .13f, .27f));
        crystal = MakeMaterial(new Color(.25f, .75f, .98f));

        RenderSettings.ambientLight = new Color(.62f, .64f, .78f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(.52f, .56f, .75f);
        RenderSettings.fogDensity = .012f;

        var sun = new GameObject("Warm Sun").AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.45f;
        sun.color = new Color(1f, .87f, .75f);
        sun.transform.rotation = Quaternion.Euler(43f, -35f, 0f);

        Block("Floating Training Island", PrimitiveType.Cube, new Vector3(0f, -1.2f, 0f),
              new Vector3(72f, 2.4f, 72f), grass);
        Block("Central Path", PrimitiveType.Cube, new Vector3(0f, .03f, 0f),
              new Vector3(7f, .07f, 50f), stone);
        for (int i = -1; i <= 1; i++)
        {
            CreateTower(new Vector3(i * 15f, 0f, 25f), i == 0 ? 1.4f : .9f);
            CreateTree(new Vector3(-19f, 0f, i * 15f - 4f));
            CreateTree(new Vector3(19f, 0f, i * 15f - 4f));
            CreateCrystal(new Vector3(-9f, .7f, i * 12f + 5f));
            CreateCrystal(new Vector3(9f, .7f, i * 12f + 5f));
        }

        var wizard = new GameObject("Player Wizard");
        wizard.transform.position = new Vector3(0f, 1.2f, -15f);
        var cc = wizard.AddComponent<CharacterController>();
        cc.height = 2.2f; cc.radius = .36f; cc.center = new Vector3(0, 1.1f, 0);
        var visual = new GameObject("Wizard Visual");
        visual.transform.SetParent(wizard.transform, false);
        var robe = Block("Robe", PrimitiveType.Capsule, new Vector3(0, 1.0f, 0), new Vector3(.83f, 1.05f, .83f), violet, visual.transform);
        var head = Block("Face", PrimitiveType.Sphere, new Vector3(0, 2.08f, 0), new Vector3(.53f, .53f, .53f), gold, visual.transform);
        Block("Wizard Hat Brim", PrimitiveType.Cylinder, new Vector3(0, 2.35f, 0), new Vector3(.82f, .09f, .82f), dark, visual.transform);
        Block("Wizard Hat", PrimitiveType.Cylinder, new Vector3(0, 2.65f, 0), new Vector3(.43f, .34f, .43f), dark, visual.transform);
        Block("Staff", PrimitiveType.Cylinder, new Vector3(.58f, 1.05f, .15f), new Vector3(.075f, .88f, .075f), bark, visual.transform);
        Block("Staff Crystal", PrimitiveType.Sphere, new Vector3(.58f, 2.0f, .15f), new Vector3(.25f, .25f, .25f), crystal, visual.transform);
        foreach (Collider c in visual.GetComponentsInChildren<Collider>()) Destroy(c);

        var mainCamera = Camera.main;
        if (mainCamera == null)
        {
            var camObject = new GameObject("Main Camera");
            camObject.tag = "MainCamera";
            mainCamera = camObject.AddComponent<Camera>();
        }
        mainCamera.fieldOfView = 65f;
        mainCamera.clearFlags = CameraClearFlags.Skybox;
        var follow = mainCamera.gameObject.AddComponent<WizardFollowCamera>();
        follow.target = wizard.transform;
        var control = wizard.AddComponent<WizardMotor>();
        control.followCamera = follow;
        wizard.AddComponent<WizardSpellCaster>();
        wizard.AddComponent<WizardMobileInput>();

        for (int i = 0; i < 3; i++)
        {
            var golem = new GameObject("Training Golem " + (i + 1));
            golem.transform.position = new Vector3((i - 1) * 6f, 0f, 9f);
            Block("Golem Torso", PrimitiveType.Cube, new Vector3(0, 1.4f, 0), new Vector3(1.3f, 1.9f, 1.1f), stone, golem.transform);
            Block("Golem Head", PrimitiveType.Cube, new Vector3(0, 2.65f, 0), new Vector3(.95f, .8f, .85f), dark, golem.transform);
            Block("Golem Eye L", PrimitiveType.Sphere, new Vector3(-.25f, 2.65f, -.46f), new Vector3(.18f, .18f, .12f), crystal, golem.transform);
            Block("Golem Eye R", PrimitiveType.Sphere, new Vector3(.25f, 2.65f, -.46f), new Vector3(.18f, .18f, .12f), crystal, golem.transform);
            var body = golem.AddComponent<BoxCollider>();
            body.center = new Vector3(0, 1.6f, 0); body.size = new Vector3(1.5f, 3.1f, 1.3f);
            golem.AddComponent<TrainingGolem>();
        }
        Debug.Log("WizardTome MVP loaded. WASD move, hold RMB orbit, Space jump, LMB/F cast.");
    }

    private static Material MakeMaterial(Color tint)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        var material = new Material(shader);
        material.color = tint;
        return material;
    }

    private static GameObject Block(string name, PrimitiveType type, Vector3 position,
                                    Vector3 scale, Material material, Transform parent = null)
    {
        var obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        if (parent != null) obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private static void CreateTower(Vector3 at, float size)
    {
        var root = new GameObject("Mage Tower");
        root.transform.position = at;
        Block("Tower Core", PrimitiveType.Cylinder, new Vector3(0, 3 * size, 0),
              new Vector3(3 * size, 3 * size, 3 * size), stone, root.transform);
        Block("Roof", PrimitiveType.Cylinder, new Vector3(0, 6.1f * size, 0),
              new Vector3(3.8f * size, .6f * size, 3.8f * size), violet, root.transform);
        Block("Beacon", PrimitiveType.Sphere, new Vector3(0, 7f * size, 0),
              Vector3.one * size, crystal, root.transform);
    }

    private static void CreateTree(Vector3 at)
    {
        var root = new GameObject("Moonwood Tree");
        root.transform.position = at;
        Block("Trunk", PrimitiveType.Cylinder, new Vector3(0, 2, 0),
              new Vector3(.7f, 2, .7f), bark, root.transform);
        Block("Crown", PrimitiveType.Sphere, new Vector3(0, 4.6f, 0),
              new Vector3(4f, 4.4f, 4f), leaf, root.transform);
    }

    private static void CreateCrystal(Vector3 at)
    {
        var obj = Block("Mana Crystal", PrimitiveType.Cube, at,
                        new Vector3(.9f, 1.4f, .9f), crystal);
        obj.transform.rotation = Quaternion.Euler(16, 35, 22);
    }
}

public sealed class WizardFollowCamera : MonoBehaviour
{
    public Transform target;
    public float yaw, pitch = 18f, distance = 6.5f;
    public Vector3 ForwardFlat
    {
        get
        {
            Vector3 f = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
            return f.normalized;
        }
    }
    private void LateUpdate()
    {
        if (target == null) return;
        if (Application.isMobilePlatform)
        {
            Vector2 drag = WizardMobileInput.CameraDelta;
            yaw += drag.x * .16f;
            pitch = Mathf.Clamp(pitch - drag.y * .16f, -8f, 65f);
        }
        else if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * 3f;
            pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 2.2f, -8f, 65f);
        }
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 focus = target.position + Vector3.up * 1.4f;
        Vector3 desired = focus - rotation * Vector3.forward * distance;
        // Prevent clipping into solid objects.
        RaycastHit hit;
        if (Physics.Linecast(focus, desired, out hit))
            desired = hit.point + hit.normal * .2f;
        transform.position = Vector3.Lerp(transform.position, desired, Mathf.Clamp01(Time.deltaTime * 12));
        transform.rotation = rotation;
    }
}

public sealed class WizardMotor : MonoBehaviour
{
    public WizardFollowCamera followCamera;
    public float speed = 5f, sprintSpeed = 8f, jumpHeight = 1.55f, gravity = -22f;
    private CharacterController character;
    private float verticalVelocity;
    private void Awake() { character = GetComponent<CharacterController>(); }
    private void Update()
    {
        float x = Application.isMobilePlatform ? WizardMobileInput.Move.x : Input.GetAxisRaw("Horizontal");
        float z = Application.isMobilePlatform ? WizardMobileInput.Move.y : Input.GetAxisRaw("Vertical");
        Vector3 input = Vector3.ClampMagnitude(new Vector3(x, 0, z), 1);
        Vector3 movement = Quaternion.Euler(0, followCamera.yaw, 0) * input;
        float currentSpeed = (Application.isMobilePlatform ? WizardMobileInput.Sprint : Input.GetKey(KeyCode.LeftShift)) ? sprintSpeed : speed;
        if (movement.sqrMagnitude > .01f)
        {
            Quaternion face = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, face, Time.deltaTime * 13f);
        }
        if (character.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        if (character.isGrounded && (Application.isMobilePlatform ? WizardMobileInput.JumpPressed : Input.GetKeyDown(KeyCode.Space)))
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        verticalVelocity += gravity * Time.deltaTime;
        character.Move((movement * currentSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.R) || transform.position.y < -15f)
        {
            character.enabled = false;
            transform.position = new Vector3(0, 1.2f, -15f);
            character.enabled = true;
            verticalVelocity = 0;
        }
    }
}

public sealed class WizardSpellCaster : MonoBehaviour
{
    public float mana = 100f, maxMana = 100f, manaRegen = 14f;
    public float cooldown = .45f, cost = 20f;
    private float nextCast;
    private void Update()
    {
        mana = Mathf.Min(maxMana, mana + manaRegen * Time.deltaTime);
        if ((Application.isMobilePlatform ? WizardMobileInput.CastPressed : (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))) &&
            Time.time >= nextCast && mana >= cost)
        {
            nextCast = Time.time + cooldown;
            mana -= cost;
            var bolt = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bolt.name = "Arcane Bolt";
            bolt.transform.localScale = Vector3.one * .42f;
            bolt.transform.position = transform.position + Vector3.up * 1.6f + transform.forward * .9f;
            var material = new Material(Shader.Find("Standard"));
            material.color = new Color(.40f, .79f, 1f);
            bolt.GetComponent<Renderer>().material = material;
            var collider = bolt.GetComponent<SphereCollider>();
            collider.isTrigger = true;
            var rb = bolt.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            var spell = bolt.AddComponent<ArcaneBolt>();
            spell.owner = transform;
            spell.direction = transform.forward;
        }
    }
    private void OnGUI()
    {
        if (Application.isMobilePlatform) return;
        GUI.Box(new Rect(16, 16, 310, 102), "WIZARDTOME  |  Training Grounds");
        GUI.Label(new Rect(30, 46, 270, 22), "Mana: " + Mathf.CeilToInt(mana) + " / " + maxMana);
        GUI.Label(new Rect(30, 69, 290, 22), "WASD move | RMB camera | Space jump");
        GUI.Label(new Rect(30, 89, 290, 22), "LMB / F cast | Shift sprint | R reset");
    }
}

public sealed class ArcaneBolt : MonoBehaviour
{
    public Vector3 direction;
    public Transform owner;
    public float speed = 23f, damage = 35f, lifetime = 2.7f;
    private void Update()
    {
        // A raycast prevents fast projectiles from passing through training targets.
        float travel = speed * Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, travel))
        {
            if (owner != null && hit.collider.transform.IsChildOf(owner))
            {
                transform.position += direction * travel;
                return;
            }
            var golem = hit.collider.GetComponentInParent<TrainingGolem>();
            if (golem != null) golem.Hit(damage);
            Destroy(gameObject);
            return;
        }
        transform.position += direction * travel;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0) Destroy(gameObject);
    }
}

public sealed class TrainingGolem : MonoBehaviour
{
    public float health = 100f;
    private float respawnAt = -1f;
    private Renderer[] visuals;
    private Collider[] colliders;
    private void Start()
    {
        visuals = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
    }
    public void Hit(float amount)
    {
        if (respawnAt >= 0) return;
        health -= amount;
        Debug.Log(name + " HP: " + Mathf.Max(0, health));
        if (health <= 0)
        {
            foreach (Renderer r in visuals) r.enabled = false;
            foreach (Collider c in colliders) c.enabled = false;
            respawnAt = Time.time + 4f;
        }
    }
    private void Update()
    {
        if (respawnAt < 0 || Time.time < respawnAt) return;
        health = 100;
        respawnAt = -1;
        foreach (Renderer r in visuals) r.enabled = true;
        foreach (Collider c in colliders) c.enabled = true;
    }
}
