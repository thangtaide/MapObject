using System;

[Serializable]
public struct ST_ObjDropResFrameTextureData
{
    public int nTi;
    public int nX;
    public int nY;
    public int nW;
    public int nH;
    public float fPx;
    public float fPy;
}




[Serializable]
public struct ST_ObjDropResTextureData
{
    public int nNumDir;
    public ST_ObjDropResFrameTextureData[] aStObjDropResFrameTextureData;
}