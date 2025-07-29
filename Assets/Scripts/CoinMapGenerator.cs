using System.Collections.Generic;
using UnityEngine;

// Create a texture and fill it with Perlin noise.
// Try varying the xOrg, yOrg and scale values in the inspector
// while in Play mode to see the effect they have on the noise.

public class CoinMapGenerator : MonoBehaviour
{
    Dictionary<float, int> coinThresholds = new Dictionary<float, int>()
    {
        {0.1f, 0}, // less than 0.1, 0 coins
        {0.2f, 1}, // 0.1 to 0.2, 1 coin
        {0.3f, 3}, // 0.2 to 0.3, 3 coins
        {0.45f, 4},
        {1, 5}
    };

    int WIDTH = 128;
    int HEIGHT = 128;
    public float scale = 20f;
    float offsetX = 0f;
    float offsetY = 0f;
    float threshold = 0.5f;

    int autoIter = 16;

    float[,] values;

    public int[,] GenerateCoinMap()
    {
        // empty grid
        values = new float[HEIGHT, WIDTH];

        // perlin values
        GenerateNoise();

        // run through the algorithm to get blobby shapes
        CellularAutomaton(autoIter);

        // average the amount of blob on each tile
        CalculateTiles();

        // decide how many coins go on the tile
        CalculateCoins();

        //PrintGrid();

        //add up the coins in the grid
        int sum = 0;
        for (int i = 0; i < values.GetLength(0); i++)
        {
            for (int j = 0; j < values.GetLength(1); j++)
            {
                sum += (int)values[i, j];
            }
        }

        //Debug.Log(sum + " " + sum / 25f);

        return GetCoinMap();
    }

    private int[,] GetCoinMap()
    {
        int[,] map = new int[5,5];

        // for each tile in the 5x5 grid
        for (int i = 0; i < values.GetLength(0); i++)
        {
            for (int j = 0; j < values.GetLength(1); j++)
            {
                map[i,j] = (int)values[i,j];
            }
        }

        return map;
    }

    private void CalculateCoins()
    {
        // for each tile in the 5x5 grid
        for (int i = 0; i < values.GetLength(0); i++)
        {
            for (int j = 0; j < values.GetLength(1); j++)
            {
                // bin each value based on a threshold
                foreach (float threshold in coinThresholds.Keys)
                {    
                    if (values[i, j] < threshold)
                    {
                        values[i, j] = coinThresholds[threshold];
                        break;
                    }
                }
            }
        }
    }

    private void CalculateTiles()
    {
        float[,] tileValues = new float[5, 5];

        int rowPixels = HEIGHT / 5;
        int colPixels = WIDTH / 5;

        // for each tile in the 5x5 grid
        for (int i = 0; i < tileValues.GetLength(0); i++)
        {
            for (int j = 0; j < tileValues.GetLength(1); j++)
            {
                // find the average of all the pixels in that tile's area
                float average = 0;
                for (int row = rowPixels * i; row < rowPixels * (i+1); row++)
                {
                    for(int col = colPixels * j; col < colPixels * (j+1); col++)
                    {
                        
                        average += values[row, col];
                    }
                }

                average /= (rowPixels * colPixels);

                tileValues[i, j] = average;
            }
        }

        values = tileValues;
    }

    public void PrintGrid()
    {
        // create a string
        string grid = "\n---------------------\n";
        for (int i = 0; i < values.GetLength(0); i++)
        {
            grid += "| ";
            for (int j = 0; j < values.GetLength(1); j++)
            {
                grid += values[i, j].ToString();            
                grid += " | ";
            }
            grid += "\n---------------------\n";
        }

        // print it out
        Debug.Log(grid);
    }

    void CellularAutomaton(int iteration)
    {
        // base case
        if (iteration <= 0)
        {
            return;
        }

        //recursive case
        //set up a new grid
        float[,] grid = new float[HEIGHT, WIDTH];

        // for each pixel, 
        for (int row = 0; row < WIDTH; row++)
        {
            for (int col = 0; col < HEIGHT; col++)
            {
                // average the values of the surrounding pixels
                float average = GetAverage(row, col);
                if (average < threshold)
                {
                    grid[row, col] = 0;
                }
                else
                {
                    grid[row, col] = 1;
                }
            }
        }

        // replace the old grid with the new one
        values = grid;

        CellularAutomaton(iteration - 1);
    }

    float GetAverage(int row, int col) 
    {
        float average = 0;
        int iter = 0;
        for(int r = row - 2; r < row + 3; r++)
        {
            for(int c = col - 2; c < col + 3; c++)
            {
                // only add cells if they are not out of bounds
                if (r > -1 && c > -1 && r < WIDTH && c < HEIGHT)
                {
                    average += values[r, c];
                }
                iter++;
            }
        }

        return average / 25;
    }

    /*
    // levels are 5 tiles long and wide, average the pixels in each tile's area, aggregating up based on iteration
    // BE CAREFUL - this will be very prone to exponential growth
    void CellularAutomaton(int iteration)
    {
        

        // base case
        if (iteration <= 0) {
            return;
        }

        // recursive case
        
        int divisions = (int)Mathf.Pow(5, iteration);

        
        
        int pixelsPerRow = WIDTH / divisions;
        int pixelsPerCol = HEIGHT / divisions;

        for (int row = 0; row < divisions; row++)
        {
            for (int col = 0; col < divisions; col++)
            {
                CalculateAverage(row, col, pixelsPerRow, pixelsPerCol);
            }
        }

        CellularAutomaton(iteration - 1);
    }

    void CalculateAverage(int row, int col, int pixelsPerRow, int pixelsPerCol)
    {
        int startRow = row * pixelsPerRow;
        int endRow = startRow + pixelsPerRow;
        int startCol = col * pixelsPerCol;
        int endCol = startCol + pixelsPerCol;

        float totalPixels = pixelsPerRow * pixelsPerCol;

        // sum up all the values in the given area
        float average = 0f;
        for (int rPixel = startRow; rPixel < endRow; rPixel++)
        {
            for (int cPixel = startCol; cPixel < endCol; cPixel++)
            {
                average += values[rPixel, cPixel];
            }
        }

        // divide by total number of pixels
        average /= totalPixels;

        // replace old values with new average
        for (int rPixel = startRow; rPixel < endRow; rPixel++)
        {
            for (int cPixel = startCol; cPixel < endCol; cPixel++)
            {
                values[rPixel, cPixel] = average;
            }
        }
    }*/

    void GenerateNoise()
    {
        // get the basic perlin noise values
        for (int row = 0; row < WIDTH; row++)
        {
            for (int col = 0; col < HEIGHT; col++)
            {
                values[row, col] = CalculateValue(row, col);
            }
        }
    }

    Texture2D CreateTexture()
    {
        Texture2D texture = new Texture2D(WIDTH, HEIGHT);

        // set each pixel in the texture to the value in the grid
        for (int row = 0; row < WIDTH; row++)
        {
            for (int col = 0; col < HEIGHT; col++)
            {
                float value = values[row, col];
                Color color = new Color(value, value, value);
                texture.SetPixel(row, col, color);
            }
        }

        texture.Apply();

        return texture;
    }

    float CalculateValue(int x, int y) {
        float xCoord = (float)x / WIDTH * scale + offsetX;
        float yCoord = (float)y / HEIGHT * scale + offsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        sample = Mathf.Clamp01(sample);

        if(sample > threshold)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
