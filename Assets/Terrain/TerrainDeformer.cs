using UnityEngine;

public class TerrainDeformer : MonoBehaviour
{
    private Terrain terr;
    protected int hmWidth; 
    protected int hmHeight; 
    private float[,] heightMapBackup;

    void Start()
    {
        terr = this.GetComponent<Terrain>();
        hmWidth = terr.terrainData.heightmapResolution;
        hmHeight = terr.terrainData.heightmapResolution;

        if (Debug.isDebugBuild)
            heightMapBackup = terr.terrainData.GetHeights(0, 0, hmWidth, hmHeight);

    }

    void OnApplicationQuit()
    {
        if (Debug.isDebugBuild)
            terr.terrainData.SetHeights(0, 0, heightMapBackup);
    }

    public void DestroyTerrain(Vector3 pos, float craterSizeInMeters)
    {
        DeformTerrain(pos, craterSizeInMeters);
    }

    protected void DeformTerrain(Vector3 pos, float craterSizeInMeters)
    {
        Vector3 terrainPos = GetRelativeTerrainPositionFromPos(pos, terr, hmWidth, hmHeight);
        int heightMapCraterWidth = (int)(craterSizeInMeters * (hmWidth / terr.terrainData.size.x));
        int heightMapCraterLength = (int)(craterSizeInMeters * (hmHeight / terr.terrainData.size.z));
        int heightMapStartPosX = (int)(terrainPos.x - (heightMapCraterWidth / 2));
        int heightMapStartPosZ = (int)(terrainPos.z - (heightMapCraterLength / 2));

        float[,] heights = terr.terrainData.GetHeights(heightMapStartPosX, heightMapStartPosZ, heightMapCraterWidth, heightMapCraterLength);
        float circlePosX;
        float circlePosY;
        float distanceFromCenter;
        float depthMultiplier;

        float deformationDepth = craterSizeInMeters / terr.terrainData.size.y;

        for (int i = 0; i < heightMapCraterLength; i++) 
        {
            for (int j = 0; j < heightMapCraterWidth; j++) 
            {
                circlePosX = (j - (heightMapCraterWidth / 2)) / (hmWidth / terr.terrainData.size.x);
                circlePosY = (i - (heightMapCraterLength / 2)) / (hmHeight / terr.terrainData.size.z);
                distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
                
                depthMultiplier = ((craterSizeInMeters / 2.0f - distanceFromCenter) / (craterSizeInMeters / 2.0f));
                depthMultiplier = Mathf.Clamp(depthMultiplier, 0, 1);
                heights[i, j] = Mathf.Clamp(heights[i, j] - deformationDepth * depthMultiplier, 0, 1);

            }
        }

        terr.terrainData.SetHeights(heightMapStartPosX, heightMapStartPosZ, heights);
    }


    protected Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos, Terrain terrain)
    {
        Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / terr.terrainData.size.x;
        coord.y = tempCoord.y / terr.terrainData.size.y;
        coord.z = tempCoord.z / terr.terrainData.size.z;

        return coord;
    }

    protected Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos, Terrain terrain, int mapWidth, int mapHeight)
    {
        Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos, terrain);
        // get the position of the terrain heightmap where this game object is
        return new Vector3((coord.x * mapWidth), 0, (coord.z * mapHeight));
    }
}