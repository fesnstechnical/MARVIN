using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineTransparent : MonoBehaviour
{
	[Header("Parameters")]
	[Range(0f, 1f)] public float m_Transparent = 0.5f;
	public Color m_OutlineColor = Color.green;
	[Range(0.01f, 0.1f)] public float m_OutlineWidth = 0.02f;
	[Range(0f, 1f)] public float m_OutlineFactor = 1f;
	[Range(0f, 0.6f)] public float m_Overlay = 0f;
	public Color m_OverlayColor = Color.red;
	[Header("Internal")]
	Renderer m_Rd;
	int m_ID_Transparent = 0;
	int m_ID_OutlineWidth = 0;
	int m_ID_OutlineColor = 0;
	int m_ID_OutlineFactor = 0;
	int m_ID_Overlay = 0;
	int m_ID_OverlayColor = 0;

    void Start ()
	{
		m_Rd = GetComponent<Renderer> ();
		m_ID_Transparent = Shader.PropertyToID ("_Transparent");
		m_ID_OutlineWidth = Shader.PropertyToID ("_OutlineWidth");
		m_ID_OutlineColor = Shader.PropertyToID ("_OutlineColor");
		m_ID_OutlineFactor = Shader.PropertyToID ("_OutlineFactor");
		m_ID_Overlay = Shader.PropertyToID ("_Overlay");
		m_ID_OverlayColor = Shader.PropertyToID ("_OverlayColor");
	}
	void Update ()
	{
		Material[] mats = m_Rd.materials;
		for (int i = 0; i < mats.Length; i++)
		{
			mats[i].SetFloat (m_ID_Transparent, m_Transparent);
			mats[i].SetFloat (m_ID_OutlineWidth, m_OutlineWidth);
			mats[i].SetColor (m_ID_OutlineColor, m_OutlineColor);
			mats[i].SetFloat (m_ID_OutlineFactor, m_OutlineFactor);
			mats[i].SetFloat (m_ID_Overlay, m_Overlay);
			mats[i].SetColor (m_ID_OverlayColor, m_OverlayColor);
		}
	}
}
