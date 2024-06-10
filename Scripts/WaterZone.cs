using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppSLZ.Rig;
using WeatherElectric.WaterDynamics.Resources;
using WeatherElectric.WaterDynamics.Scripts.BodyManagers;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace WeatherElectric.WaterDynamics.Scripts;

public class WaterZone : MonoBehaviour
{
    private readonly List<RbBuoyancyManager> _inTriggerCol = [];

    #region Water Settings

    public Il2CppValueField<float> BuoyancyMultiplier = 1.0f;
    public Il2CppValueField<float> Midpoint = 50.0f;
    public Il2CppValueField<bool> MidpointSink = true;
    public Il2CppValueField<bool> Dampening = true;
    public Il2CppValueField<float> DampeningAmount = 0.98f;
    public Il2CppValueField<bool> IgnoreRigManager = false;

    #endregion

    #region Swimming

    public Il2CppValueField<bool> AllowSwimming = true;
    public Il2CppValueField<float> MinimumVelocity = 10f;
    public Il2CppValueField<float> VelocityMultiplier = 100f;

    #endregion

    #region Particle Settings

    public Il2CppValueField<bool> EnableSplashParticles = true;
    public Il2CppReferenceField<GameObject> SplashParticlePrefab = Assets.DefaultSplashParticles;
    public Il2CppValueField<float> SplashParticleLifetime = 15.0f;

    #endregion

    private void Awake()
    {
        SplashParticlePrefab ??= Assets.DefaultSplashParticles;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        var colliderRigidbody = other.attachedRigidbody;
        var rigManager = other.GetComponentInParent<RigManager>();
        if (rigManager != null && IgnoreRigManager) return;
        AddHandMonitors(rigManager);
        if (EnableSplashParticles)
        {
            var splashParticle = Instantiate(SplashParticlePrefab, other.ClosestPoint(transform.position), Quaternion.identity);
            Destroy(splashParticle, SplashParticleLifetime);
        }
        RbBuoyancyManager buoyancyManager = null;
        if (colliderRigidbody != null) buoyancyManager = colliderRigidbody.GetComponent<RbBuoyancyManager>();
        if (buoyancyManager != null)
        {
            buoyancyManager.enabled = true;
            _inTriggerCol.Add(buoyancyManager);
            return;
        }

        if (colliderRigidbody != null && colliderRigidbody.GetComponent<RbBuoyancyManager>() == null)
        {

            var addedManager = colliderRigidbody.gameObject.AddComponent<RbBuoyancyManager>();
            addedManager.dampening = Dampening;
            addedManager.buoyancyMultiplier = BuoyancyMultiplier;
            addedManager.midpoint = Midpoint;
            addedManager.dampeningAmount = DampeningAmount;
            addedManager.midpointSink = MidpointSink;
            addedManager.OnDestroyed = OnBuoyancyManagerDestroyed;
            Rigidbody[] childRbs = colliderRigidbody.GetComponentsInChildren<Rigidbody>(true);
            foreach (var rb in childRbs)
            {
                if (rb.isKinematic)
                {
                    if (rb.GetComponent<RbBuoyancyManager>() == null)
                    {
                        var manager = rb.gameObject.AddComponent<RbBuoyancyManager>();
                        manager.buoyancyMultiplier = BuoyancyMultiplier;
                        manager.dampening = Dampening;
                        manager.midpoint = Midpoint;
                        manager.dampeningAmount = DampeningAmount;
                        manager.midpointSink = MidpointSink;
                    }
                }
            } 

            _inTriggerCol.Add(addedManager);
        }
    }

    private void AddHandMonitors(RigManager rigManager)
    {
        if (rigManager == null) return;
        if (AllowSwimming)
        {
            if (rigManager.physicsRig.leftHand.GetComponent<HandMonitor>() == null)
            {
                var leftHandMonitor = rigManager.physicsRig.leftHand.gameObject.AddComponent<HandMonitor>();
                leftHandMonitor.RigManager = rigManager;
                leftHandMonitor.MinVelocity = MinimumVelocity;
                leftHandMonitor.VelocityMult = VelocityMultiplier;
            }

            if (rigManager.physicsRig.rightHand.GetComponent<HandMonitor>() == null)
            {
                var rightHandMonitor = rigManager.physicsRig.rightHand.gameObject.AddComponent<HandMonitor>();
                rightHandMonitor.RigManager = rigManager;
                rightHandMonitor.MinVelocity = MinimumVelocity;
                rightHandMonitor.VelocityMult = VelocityMultiplier;
            }
        }
    }
    
    private void RemoveHandMonitors(RigManager rigManager)
    {
        if (rigManager == null) return;
        if (AllowSwimming)
        {
            if (rigManager.physicsRig.leftHand.GetComponent<HandMonitor>() != null)
            {
                Destroy(rigManager.physicsRig.leftHand.GetComponent<HandMonitor>());
            }

            if (rigManager.physicsRig.rightHand.GetComponent<HandMonitor>() != null)
            {
                Destroy(rigManager.physicsRig.rightHand.GetComponent<HandMonitor>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        var rigManager = other.GetComponentInParent<RigManager>();
        RemoveHandMonitors(rigManager);
        var colliderRigidbody = other.attachedRigidbody;
        RbBuoyancyManager manager = null;

        if (colliderRigidbody != null) manager = colliderRigidbody.GetComponent<RbBuoyancyManager>();

        if (_inTriggerCol.Contains(manager))
        {
            colliderRigidbody.useGravity = true;
            Destroy(manager);
            Rigidbody[] childRbs = colliderRigidbody.GetComponentsInChildren<Rigidbody>(true);
            foreach (var rb in childRbs)
                if (rb.isKinematic)
                    if (rb.GetComponent<RbBuoyancyManager>() != null)
                        Destroy(rb.GetComponent<RbBuoyancyManager>());
        }
    }

    [HideFromIl2Cpp]
    private void OnBuoyancyManagerDestroyed(RbBuoyancyManager manager)
    {
        _inTriggerCol.Remove(manager);
    }
    
    public WaterZone(IntPtr ptr) : base(ptr) { }
}