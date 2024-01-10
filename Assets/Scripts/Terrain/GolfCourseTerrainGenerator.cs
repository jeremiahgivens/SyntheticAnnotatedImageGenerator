using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfCourseTerrainGenerator : MonoBehaviour
{
    public Terrain m_Terrain;

    public float m_PerlinHeightMultiplier = 0.012f;
    public float m_PerlinNodeSpacingMultiplier = 0.1f;
    
    public float m_PerlinNoiseHeightMultiplier = 0.1f;
    public float m_PerlinNoiseNodeSpacingMultiplier = 0.01f;

    public bool m_DoGenerateTerrain = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DoGenerateTerrain)
        {
            m_DoGenerateTerrain = false;
            GenerateTerrain();
        }
    }

    public void GenerateTerrain()
    {
        int res = 513;
        var heightmap = new float[res, res];

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                heightmap[x, y] =
                    Mathf.PerlinNoise(x * m_PerlinNodeSpacingMultiplier, y * m_PerlinNodeSpacingMultiplier) *
                    m_PerlinHeightMultiplier + 
                    Mathf.PerlinNoise(x * m_PerlinNoiseNodeSpacingMultiplier, y * m_PerlinNoiseNodeSpacingMultiplier) *
                    m_PerlinNoiseHeightMultiplier;
            }
        }
        
        m_Terrain.terrainData.SetHeights(0, 0, heightmap);
    }
}
