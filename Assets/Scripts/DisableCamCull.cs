using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering;

public class DisableCamCull : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = this.GetComponent<Camera>();
    }

    void OnPreCull()
    {
        cam.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) *
            Matrix4x4.Translate(Vector3.forward * -99999 / 2f) * cam.worldToCameraMatrix;

        /*
        cam.ResetCullingMatrix();
        Matrix4x4 m = cam.cullingMatrix;
        m.SetRow(0, m.GetRow(0) * 0.5f);
        m.SetRow(1, m.GetRow(1) * 0.5f);
        cam.cullingMatrix = m;
         */
    }

    void OnDisable()
    {
        cam.ResetCullingMatrix();
    }

    /*
    private void Start()
    {
        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
    }
 
    private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        Camera.main.cullingMatrix = Matrix4x4.Ortho(-99999, 99999, -99999, 99999, 0.001f, 99999) * 
            Matrix4x4.Translate(Vector3.forward * -99999 / 2f) * Camera.main.worldToCameraMatrix;
    }
     */
}
