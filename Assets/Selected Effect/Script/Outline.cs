using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Outline : MonoBehaviour
{
    [SerializeField]
    public bool isColliding = false;

    public enum ETriggerMethod { MouseMove = 0, MouseRightPress, MouseLeftPress, AlwaysOn, Collider };
	[Header("Trigger Method")]
	public ETriggerMethod m_TriggerMethod = ETriggerMethod.MouseMove;
	public bool m_Persistent = false;
	[Header("Normal Expansion Parameters")]
	public Color m_OutlineColor = Color.green;
	[Range(0.01f, 0.1f)] public float m_OutlineWidth = 0.02f;
	[Range(0f, 1f)] public float m_OutlineFactor = 1f;
	public bool m_WriteZ = false;
	public bool m_BasedOnVertexColorR = false;
	public bool m_OutlineOnly = false;
	[Range(-16f, -1f)] public float m_DepthOffset = -8f;
	[Header("Normal Expansion Flash")]
	public Color m_OverlayColor = Color.red;
	[Range(0f, 0.6f)] public float m_Overlay = 0f;
	public bool m_OverlayFlash = false;
	[Range(1f, 6f)] public float m_OverlayFlashSpeed = 3f;
	[Header("Internal")]
	public Material[] m_BackupMats;
	public Renderer m_Rd;
	public Shader m_SdrOutlineOnly, m_SdrOutlineDiffuse, m_SdrOriginal;
	public OutlineMgr m_Mgr;
	bool m_IsMouseOn = false;

    

    public void Initialize ()
	{
		m_Mgr = GameObject.FindObjectOfType<OutlineMgr> ();
		m_Rd = GetComponent<Renderer> ();
		m_BackupMats = m_Rd.materials;
		m_SdrOriginal = Shader.Find ("Standard");
		m_SdrOutlineOnly = Shader.Find ("Selected Effect --- Outline/Normal Expansion/Outline Only");
		m_SdrOutlineDiffuse = Shader.Find ("Selected Effect --- Outline/Normal Expansion/Diffuse");
	}
	public void UpdateSelfParameters ()
	{
		// trigger effect logic
		if (m_TriggerMethod == ETriggerMethod.MouseRightPress)
		{
			bool on = m_IsMouseOn && Input.GetMouseButton (1);
			if (on)
				OutlineEnable ();
			else
				OutlineDisable ();
		}
		else if (m_TriggerMethod == ETriggerMethod.MouseLeftPress)
		{
			bool on = m_IsMouseOn && Input.GetMouseButton (0);
			if (on)
				OutlineEnable ();
			else
				OutlineDisable ();
		}
        else if (m_TriggerMethod == ETriggerMethod.AlwaysOn)
        {
            OutlineEnable();
        }
        else if (m_TriggerMethod == ETriggerMethod.Collider)
        {
            if(isColliding)
            {
                OutlineEnable();
            }
            else
            {
                OutlineDisable();
            }
        }
		
		// material effect parameters
		if (m_OverlayFlash)
		{
			float curve = Mathf.Sin (Time.time * m_OverlayFlashSpeed) * 0.5f + 0.5f;
			m_Overlay = curve * 0.6f;
		}
		Material[] mats = m_Rd.materials;
		for (int i = 0; i < mats.Length; i++)
		{
			mats[i].SetFloat ("_OutlineWidth", m_OutlineWidth);
			mats[i].SetColor ("_OutlineColor", m_OutlineColor);
			mats[i].SetFloat ("_OutlineFactor", m_OutlineFactor);
			mats[i].SetColor ("_OverlayColor", m_OverlayColor);
			mats[i].SetTexture ("_MainTex", m_BackupMats[i].GetTexture ("_MainTex"));
			mats[i].SetTextureOffset ("_MainTex", m_BackupMats[i].GetTextureOffset ("_MainTex"));
			mats[i].SetTextureScale ("_MainTex", m_BackupMats[i].GetTextureScale ("_MainTex"));
			mats[i].SetFloat ("_OutlineWriteZ", m_WriteZ ? 1f : 0f);
			mats[i].SetFloat ("_OutlineBasedVertexColorR", m_BasedOnVertexColorR ? 0f : 1f);
			mats[i].SetFloat ("_Overlay", m_Overlay);
			mats[i].SetFloat ("_DepthOffset", m_DepthOffset);
		}
	}
	void OutlineEnable ()
	{
		if (m_Mgr.m_Tech == OutlineMgr.ETech.NormalExpansion)
		{
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
			{
				if (m_OutlineOnly)
					mats[i].shader = m_SdrOutlineOnly;
				else
					mats[i].shader = m_SdrOutlineDiffuse;
			}
		}
		else if (m_Mgr.m_Tech == OutlineMgr.ETech.PostProcess)
		{
			gameObject.layer = LayerMask.NameToLayer (m_Mgr.m_Layer);
		}
	}
	void OutlineDisable ()
	{
		if (m_Mgr.m_Tech == OutlineMgr.ETech.NormalExpansion)
		{
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
				mats[i].shader = m_SdrOriginal;
		}
		else if (m_Mgr.m_Tech == OutlineMgr.ETech.PostProcess)
		{
			gameObject.layer = LayerMask.NameToLayer ("Default");
		}
	}
	void OnMouseEnter ()
	{
		m_IsMouseOn = true;
		if (m_TriggerMethod == ETriggerMethod.MouseMove)
			OutlineEnable ();
	}
	void OnMouseExit ()
	{
		m_IsMouseOn = false;
		if (!m_Persistent)
			OutlineDisable ();
	}
	public void DisableFx ()
	{
		OutlineDisable ();
		gameObject.layer = LayerMask.NameToLayer ("Default");
	}
}
