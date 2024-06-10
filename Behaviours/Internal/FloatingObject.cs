namespace WeatherElectric.WaterDynamics.Behaviours.Internal;

public class FloatingObject : MonoBehaviour
{
    // All of this is NonSerialized because Interop throws an error if a monobehaviour has public fields that aren't Il2CppValueField<>.
    [NonSerialized] 
    private Rigidbody _thisRb;
    [NonSerialized] 
    public float BuoyancyMultiplier;
    [NonSerialized] 
    public float Midpoint;
    [NonSerialized] 
    public bool MidpointSink;
    [NonSerialized] 
    public bool Dampening;
    [NonSerialized] 
    public float DampeningAmount;

    [NonSerialized] 
    internal Action<FloatingObject> OnDestroyed = null;

    private void Start()
    {
        _thisRb = GetComponent<Rigidbody>();
        if (_thisRb != null) _thisRb.useGravity = false;
    }

    private void OnDisable()
    {
        if (_thisRb != null) _thisRb.useGravity = true;

        Destroy(this);
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }

    private void FixedUpdate()
    {
        if (_thisRb && _thisRb.useGravity) _thisRb.useGravity = false;
        if (_thisRb && _thisRb.mass < Midpoint)
        {
            var buoyantForce = _thisRb.mass + Physics.gravity.magnitude * BuoyancyMultiplier;
            _thisRb.AddForce(Vector3.up * buoyantForce);
            if (Dampening) _thisRb.velocity *= DampeningAmount;
        }

        if (_thisRb && _thisRb.mass > Midpoint)
        {
            var buoyantForce = _thisRb.mass + Physics.gravity.magnitude * BuoyancyMultiplier;
            _thisRb.AddForce(Vector3.down * buoyantForce);
            if (Dampening) _thisRb.velocity *= DampeningAmount;
        }

        if (_thisRb && Mathf.Approximately(_thisRb.mass, Midpoint))
            switch (MidpointSink)
            {
                case true:
                {
                    var buoyantForce = _thisRb.mass + Physics.gravity.magnitude * BuoyancyMultiplier;
                    _thisRb.AddForce(Vector3.down * buoyantForce);
                    if (Dampening) _thisRb.velocity *= DampeningAmount;

                    break;
                }
                case false:
                {
                    var buoyantForce = _thisRb.mass + Physics.gravity.magnitude * BuoyancyMultiplier;
                    _thisRb.AddForce(Vector3.up * buoyantForce);
                    if (Dampening) _thisRb.velocity *= DampeningAmount;

                    break;
                }
            }
    }
    
    public FloatingObject(IntPtr ptr) : base(ptr) { }
}