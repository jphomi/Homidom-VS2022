package crc641413d26d4b4993bb;


public class Zone
	extends crc641413d26d4b4993bb.BaseObject
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_hashCode:()I:GetGetHashCodeHandler\n" +
			"n_toString:()Ljava/lang/String;:GetToStringHandler\n" +
			"";
		mono.android.Runtime.register ("HoMIDroid.BO.Zone, HoMIDroid", Zone.class, __md_methods);
	}


	public Zone ()
	{
		super ();
		if (getClass () == Zone.class)
			mono.android.TypeManager.Activate ("HoMIDroid.BO.Zone, HoMIDroid", "", this, new java.lang.Object[] {  });
	}


	public int hashCode ()
	{
		return n_hashCode ();
	}

	private native int n_hashCode ();


	public java.lang.String toString ()
	{
		return n_toString ();
	}

	private native java.lang.String n_toString ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
