namespace WeatherElectric.WaterDynamics.Scripts.BodyManagers;

public class RbBuoyancyManager : MonoBehaviour
{
    public Rigidbody thisRb;
    public float buoyancyMultiplier;
    public float midpoint;
    public bool midpointSink;
    public bool dampening;
    public float dampeningAmount;

    [NonSerialized] internal Action<RbBuoyancyManager> OnDestroyed = null;

    private void Start()
    {
        thisRb = GetComponent<Rigidbody>();
        if (thisRb != null) thisRb.useGravity = false;
    }

    private void OnDisable()
    {
        if (thisRb != null) thisRb.useGravity = true;

        Destroy(this);
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }

    private void FixedUpdate()
    {
        if (thisRb && thisRb.useGravity) thisRb.useGravity = false;
        if (thisRb && thisRb.mass < midpoint)
        {
            var buoyantForce = thisRb.mass + Physics.gravity.magnitude * buoyancyMultiplier;
            thisRb.AddForce(Vector3.up * buoyantForce);
            if (dampening) thisRb.velocity *= dampeningAmount;
        }

        if (thisRb && thisRb.mass > midpoint)
        {
            var buoyantForce = thisRb.mass + Physics.gravity.magnitude * buoyancyMultiplier;
            thisRb.AddForce(Vector3.down * buoyantForce);
            if (dampening) thisRb.velocity *= dampeningAmount;
        }

        if (thisRb && Mathf.Approximately(thisRb.mass, midpoint))
            switch (midpointSink)
            {
                case true:
                {
                    var buoyantForce = thisRb.mass + Physics.gravity.magnitude * buoyancyMultiplier;
                    thisRb.AddForce(Vector3.down * buoyantForce);
                    if (dampening) thisRb.velocity *= dampeningAmount;

                    break;
                }
                case false:
                {
                    var buoyantForce = thisRb.mass + Physics.gravity.magnitude * buoyancyMultiplier;
                    thisRb.AddForce(Vector3.up * buoyantForce);
                    if (dampening) thisRb.velocity *= dampeningAmount;

                    break;
                }
            }
    }
    
    public RbBuoyancyManager(IntPtr ptr) : base(ptr) { }
}