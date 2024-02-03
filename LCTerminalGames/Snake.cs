using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace LCTerminalGames;

public class Snake
{
    private static Vector2Int snakeHead;
    private static Vector2Int apple;
    private static Vector2Int direction;
    private static ArrayList snakeBody = new ArrayList();
    private static int width = 15;
    private static int height = 15;
    private static int highScore = 0;

    public static void RunGame(Action<string> setScreen)
    {
        UpdateGameState();
        RenderGameState(setScreen);
    }

    public static void StartGame()
    {
        snakeHead = new Vector2Int(width / 2, height / 2);
        snakeBody.Clear();
        snakeBody.Add(snakeHead);
        direction = Vector2Int.right;
        SpawnApple();
    }

    private static void UpdateGameState()
    {
        Vector2Int newHead = snakeHead + direction;
        snakeHead = newHead;
        snakeBody.Insert(0, newHead);

        if (newHead == apple)
        {
            SpawnApple();
        }
        else
        {
            snakeBody.RemoveAt(snakeBody.Count - 1);
        }

        if (newHead.x < 0 || newHead.x >= width || newHead.y < 0 || newHead.y >= height)
        {
            StartGame();
        }

        foreach (Vector2Int part in snakeBody.GetRange(1, snakeBody.Count - 1))
        {
            if (part == newHead)
            {
                StartGame();
                break;
            }
        }
    }

    private static void RenderGameState(Action<string> setScreen)
    {
        char[][] grid = new char[height][];

        for (int y = 0; y < height; y++)
        {
            grid[y] = new char[width];
            for (int x = 0; x < width; x++)
            {
                grid[y][x] = '.';
            }
        }

        foreach (Vector2Int part in snakeBody)
        {
            grid[part.y][part.x] = 'S';
        }

        grid[apple.y][apple.x] = 'A';

        if (snakeBody.Count > highScore)
            highScore = snakeBody.Count;
        string display = $"Points: {snakeBody.Count}\nHigh Score: {highScore}\n";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                display += grid[y][x] + " ";
            }

            display += "\n";
        }

        setScreen(display);
    }

    private static void SpawnApple()
    {
        do
        {
            apple = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (snakeBody.Contains(apple));
    }

    public static void HandleKey(KeyCode keyCode)
    {
            if (keyCode == KeyCode.UpArrow || keyCode == KeyCode.W)
                direction = new Vector2Int(0, -1);
            else if (keyCode == KeyCode.DownArrow || keyCode == KeyCode.S)
                direction = new Vector2Int(0, 1);
            else if (keyCode == KeyCode.LeftArrow || keyCode == KeyCode.A)
                direction = new Vector2Int(-1, 0);
            else if (keyCode == KeyCode.RightArrow || keyCode == KeyCode.D)
                direction = new Vector2Int(1, 0);
    }
}