using Il2CppSLZ.Marrow;

namespace WeatherElectric.WaterDynamics.Behaviours.Internal;

public class HandSwimmingController : MonoBehaviour
{
    private Vector3 _velocity;
    
    internal RigManager RigManager;
    internal float MinVelocity;
    internal float VelocityMult;
    
    private Rigidbody _handRb;
    private Rigidbody _chestRb;
    private Rigidbody _headRb;
    

    private void Start()
    {
        _headRb = RigManager.physicsRig.m_head.GetComponent<Rigidbody>();
        _chestRb = RigManager.physicsRig.m_chest.GetComponent<Rigidbody>();
        _handRb = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate()
    {
        _velocity = _handRb.velocity - _chestRb.velocity;
        if (_velocity.sqrMagnitude > MinVelocity)
        {
            _headRb.AddRelativeForce(new Vector3(0, 0, _velocity.sqrMagnitude * VelocityMult));
        }
    }
    
    public HandSwimmingController(IntPtr ptr) : base(ptr) { }
}