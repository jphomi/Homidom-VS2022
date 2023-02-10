package crc64da21ca3ef8c6d683;


public abstract class BaseExpandableGroupAdapter_1
	extends android.widget.BaseExpandableListAdapter
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getGroupCount:()I:GetGetGroupCountHandler\n" +
			"n_hasStableIds:()Z:GetHasStableIdsHandler\n" +
			"n_isChildSelectable:(II)Z:GetIsChildSelectable_IIHandler\n" +
			"n_getChild:(II)Ljava/lang/Object;:GetGetChild_IIHandler\n" +
			"n_getChildId:(II)J:GetGetChildId_IIHandler\n" +
			"n_getChildrenCount:(I)I:GetGetChildrenCount_IHandler\n" +
			"n_getGroup:(I)Ljava/lang/Object;:GetGetGroup_IHandler\n" +
			"n_getGroupId:(I)J:GetGetGroupId_IHandler\n" +
			"";
		mono.android.Runtime.register ("HoMIDroid.Adapters.BaseExpandableGroupAdapter`1, HoMIDroid", BaseExpandableGroupAdapter_1.class, __md_methods);
	}


	public BaseExpandableGroupAdapter_1 ()
	{
		super ();
		if (getClass () == BaseExpandableGroupAdapter_1.class)
			mono.android.TypeManager.Activate ("HoMIDroid.Adapters.BaseExpandableGroupAdapter`1, HoMIDroid", "", this, new java.lang.Object[] {  });
	}


	public int getGroupCount ()
	{
		return n_getGroupCount ();
	}

	private native int n_getGroupCount ();


	public boolean hasStableIds ()
	{
		return n_hasStableIds ();
	}

	private native boolean n_hasStableIds ();


	public boolean isChildSelectable (int p0, int p1)
	{
		return n_isChildSelectable (p0, p1);
	}

	private native boolean n_isChildSelectable (int p0, int p1);


	public java.lang.Object getChild (int p0, int p1)
	{
		return n_getChild (p0, p1);
	}

	private native java.lang.Object n_getChild (int p0, int p1);


	public long getChildId (int p0, int p1)
	{
		return n_getChildId (p0, p1);
	}

	private native long n_getChildId (int p0, int p1);


	public int getChildrenCount (int p0)
	{
		return n_getChildrenCount (p0);
	}

	private native int n_getChildrenCount (int p0);


	public java.lang.Object getGroup (int p0)
	{
		return n_getGroup (p0);
	}

	private native java.lang.Object n_getGroup (int p0);


	public long getGroupId (int p0)
	{
		return n_getGroupId (p0);
	}

	private native long n_getGroupId (int p0);

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
