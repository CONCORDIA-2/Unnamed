using UnityEngine;

namespace Assets.Scripts.Cam.Effects {
	
	[ExecuteInEditMode]
	[RequireComponent(typeof(UnityEngine.Camera))]

	public class CrushLights : MonoBehaviour {

		private Material m_material;
		private Shader shader;

		//assigned on startup
		public float darknessThreshold = 0.03f;
		public float brightnessThreshold = 0.9f;

		public bool isRaven = true;

		void Start() {
			isRaven = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<LocalPlayerManager>().IsRaven();
		}

		private Material material {
			get {
				if (m_material == null) {
					shader = Shader.Find("CrushLights");
					m_material = new Material(shader) {hideFlags = HideFlags.DontSave};

					//get client--
					if (isRaven) m_material.SetFloat("_IsRaven", 1.0f);
					else m_material.SetFloat("_IsRaven", 0.0f);
					m_material.SetFloat("_DarkThreshold", darknessThreshold);
					m_material.SetFloat("_BrightThreshold", brightnessThreshold);
				}
				return m_material;
			}
		}

		public void OnRenderImage(RenderTexture src, RenderTexture dest) {
			if (material) Graphics.Blit(src, dest, material);
		}
	}
}