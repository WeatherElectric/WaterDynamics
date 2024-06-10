using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppSLZ.Rig;

// ReSharper disable UnassignedField.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace WeatherElectric.WaterDynamics.Behaviours;

public class WaterZone : MonoBehaviour
{
    private readonly List<FloatingObject> _inTriggerCol = [];

    #region Water Settings
    
    public Il2CppValueField<float> BuoyancyMultiplier;
    public Il2CppValueField<float> Midpoint;
    public Il2CppValueField<bool> MidpointSink;
    public Il2CppValueField<bool> Dampening;
    public Il2CppValueField<float> DampeningAmount;
    public Il2CppValueField<bool> IgnoreRigManager;

    #endregion

    #region Swimming

    public Il2CppValueField<bool> AllowSwimming;
    public Il2CppValueField<float> MinimumVelocity;
    public Il2CppValueField<float> VelocityMultiplier;

    #endregion

    #region Particle Settings

    public Il2CppValueField<bool> EnableSplashParticles;
    public Il2CppReferenceField<GameObject> SplashParticlePrefab;
    public Il2CppValueField<float> SplashParticleLifetime;
    public Il2CppValueField<float> MinimumVelocityForSplash;
    
    #endregion

    private void Start()
    {
        if (SplashParticlePrefab.Get() == null) SplashParticlePrefab.Set(Assets.DefaultSplashParticles);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        var colliderRigidbody = other.attachedRigidbody;
        var rigManager = other.GetComponentInParent<RigManager>();
        if (rigManager != null && IgnoreRigManager.Get()) return;
        AddHandMonitors(rigManager);
        SpawnSplashParticle(other);
        FloatingObject buoyancyManager = null;
        if (colliderRigidbody != null) buoyancyManager = colliderRigidbody.GetComponent<FloatingObject>();
        if (buoyancyManager != null)
        {
            buoyancyManager.enabled = true;
            _inTriggerCol.Add(buoyancyManager);
            return;
        }

        if (colliderRigidbody != null && colliderRigidbody.GetComponent<FloatingObject>() == null)
        {

            var addedManager = colliderRigidbody.gameObject.AddComponent<FloatingObject>();
            addedManager.Dampening = Dampening;
            addedManager.BuoyancyMultiplier = BuoyancyMultiplier;
            addedManager.Midpoint = Midpoint;
            addedManager.DampeningAmount = DampeningAmount;
            addedManager.MidpointSink = MidpointSink;
            addedManager.OnDestroyed = OnBuoyancyManagerDestroyed;
            Rigidbody[] childRbs = colliderRigidbody.GetComponentsInChildren<Rigidbody>(true);
            foreach (var rb in childRbs)
            {
                if (rb.isKinematic)
                {
                    if (rb.GetComponent<FloatingObject>() == null)
                    {
                        var manager = rb.gameObject.AddComponent<FloatingObject>();
                        manager.BuoyancyMultiplier = BuoyancyMultiplier;
                        manager.Dampening = Dampening;
                        manager.Midpoint = Midpoint;
                        manager.DampeningAmount = DampeningAmount;
                        manager.MidpointSink = MidpointSink;
                    }
                }
            } 

            _inTriggerCol.Add(addedManager);
        }
    }

    private void SpawnSplashParticle(Collider other)
    {
        if (EnableSplashParticles.Get() == false) return;
        if (other.attachedRigidbody.velocity.sqrMagnitude < MinimumVelocityForSplash) return;
        Vector3 pos = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);
        var splashParticle = Instantiate(SplashParticlePrefab.Get(), pos, Quaternion.identity);
        Destroy(splashParticle, SplashParticleLifetime.Get());
    }

    private void AddHandMonitors(RigManager rigManager)
    {
        if (rigManager == null) return;
        if (AllowSwimming.Get())
        {
            if (rigManager.physicsRig.leftHand.GetComponent<HandSwimmingController>() == null)
            {
                var leftHandMonitor = rigManager.physicsRig.leftHand.gameObject.AddComponent<HandSwimmingController>();
                leftHandMonitor.RigManager = rigManager;
                leftHandMonitor.MinVelocity = MinimumVelocity;
                leftHandMonitor.VelocityMult = VelocityMultiplier;
            }

            if (rigManager.physicsRig.rightHand.GetComponent<HandSwimmingController>() == null)
            {
                var rightHandMonitor = rigManager.physicsRig.rightHand.gameObject.AddComponent<HandSwimmingController>();
                rightHandMonitor.RigManager = rigManager;
                rightHandMonitor.MinVelocity = MinimumVelocity;
                rightHandMonitor.VelocityMult = VelocityMultiplier;
            }
        }
    }
    
    private void RemoveHandMonitors(RigManager rigManager)
    {
        if (rigManager == null) return;
        if (AllowSwimming.Get())
        {
            if (rigManager.physicsRig.leftHand.GetComponent<HandSwimmingController>() != null)
            {
                Destroy(rigManager.physicsRig.leftHand.GetComponent<HandSwimmingController>());
            }

            if (rigManager.physicsRig.rightHand.GetComponent<HandSwimmingController>() != null)
            {
                Destroy(rigManager.physicsRig.rightHand.GetComponent<HandSwimmingController>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
        var rigManager = other.GetComponentInParent<RigManager>();
        RemoveHandMonitors(rigManager);
        var colliderRigidbody = other.attachedRigidbody;
        FloatingObject manager = null;

        if (colliderRigidbody != null) manager = colliderRigidbody.GetComponent<FloatingObject>();

        if (_inTriggerCol.Contains(manager))
        {
            colliderRigidbody.useGravity = true;
            Destroy(manager);
            Rigidbody[] childRbs = colliderRigidbody.GetComponentsInChildren<Rigidbody>(true);
            foreach (var rb in childRbs)
                if (rb.isKinematic)
                    if (rb.GetComponent<FloatingObject>() != null)
                        Destroy(rb.GetComponent<FloatingObject>());
        }
    }

    [HideFromIl2Cpp]
    private void OnBuoyancyManagerDestroyed(FloatingObject manager)
    {
        _inTriggerCol.Remove(manager);
    }
    
    public WaterZone(IntPtr ptr) : base(ptr) { }
}