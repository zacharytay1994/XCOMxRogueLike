using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding
{
    public PathfindingGrid grid_ = new PathfindingGrid();
    List<Node> nodes_to_reset_ = new List<Node>();

    public Pathfinding()
    {
    }

    public bool FindPath(Vector2Int gridstart, Vector2Int gridend, ref List<Node> pathref)
    {
        // create open set
        List<Node> open_set = new List<Node>();

        Node start_node = grid_.GetNode(gridstart.x, gridstart.y);
        Node end_node = grid_.GetNode(gridend.x, gridend.y);

        open_set.Add(start_node);
        start_node.inOpen_ = true;
        nodes_to_reset_.Add(start_node);

        while (open_set.Count > 0)
        {
            Node current_node = open_set[GetLowestFNode(open_set)];

            // erase current node from open set
            open_set.Remove(current_node);
            // remove current node from open set
            current_node.inOpen_ = false;
            // add current node to close set by flag
            current_node.inClosed_ = true;

            // if current node equals to end node, we have reached destination
            if (current_node == end_node)
            {
                pathref = RetracePath(start_node, end_node);
                return true;
            }

            // else continue search
            foreach (Node n in grid_.GetNeighbours(current_node))
            {
                // if not traversable or in closed set, skip
                if (!n.isWalkable_ || n.inClosed_)
                {
                    continue;
                }
                // if new g cost < g cost (need updating), or if not in open set, update/calculate f cost, add to open set
                // calculate new g cost of neighbour relative to current node
                int new_g_cost = current_node.gCost_ + GetDistanceBetweenNodes(current_node, n);
                if (new_g_cost < n.gCost_ || !n.inOpen_)
                {
                    n.gCost_ = new_g_cost;
                    n.hCost_ = GetDistanceBetweenNodes(n, end_node);
                    n.parent_ = current_node;
                    if (!n.inOpen_)
                    {
                        open_set.Add(n);
                        n.inOpen_ = true;
                    }
                    nodes_to_reset_.Add(n);
                }
            }
        }
        return false;
     }

    public int GetDistanceBetweenNodes(Node node1, Node node2)
    {
        int distance_x = Math.Abs(node1.gridX_ - node2.gridX_);
        int distance_y = Math.Abs(node1.gridY_ - node2.gridY_);

        if (distance_x > distance_y)
        {
            return distance_y * 14 + (distance_x - distance_y) * 10;
        }
        return distance_x * 14 + (distance_y - distance_x) * 10;
    }

    public int GetLowestFNode(List<Node> list)
    {
        int size = list.Count;
        int lowest = 0;
        for (int i = 0; i < size; i++)
        {
            if (list[lowest].GetFCost() > list[i].GetFCost())
            {
                lowest = i;
            }
        }
        return lowest;
    }

    public List<Node> RetracePath(Node startnode, Node endnode)
    {
        List<Node> path = new List<Node>();
        Node current_node = endnode;

        // trace parent back to find path
        while (startnode != current_node)
        {
            path.Add(current_node);
            current_node = current_node.parent_;
        }

        // reverse path from back to front
        path.Reverse();
        // reset all tempered nodes
        foreach (Node n in nodes_to_reset_)
        {
            n.Reset();
        }
        nodes_to_reset_.Clear();
        return path;
    }
}



public class Node
{
    public int gCost_ = 0;
    public int hCost_ = 0;
    public readonly int gridX_;
    public readonly int gridY_;

    public bool isWalkable_;
    public Node parent_;
    public bool inClosed_ = false;
    public bool inOpen_ = false;

    public Node(bool iswalkable, int gridx, int gridy)
    {
        isWalkable_ = iswalkable;
        gridX_ = gridx;
        gridY_ = gridy;
    }

    public int GetFCost()
    {
        return gCost_ + hCost_;
    }

    public void Reset()
    {
        gCost_ = 0;
        hCost_ = 0;
        inClosed_ = false;
        inOpen_ = false;
    }
}

public class PathfindingGrid
{
    Node[,] node_grid_;
    public int grid_size_x;
    public int grid_size_y;

    public PathfindingGrid()
    {}

    public void InitGrid(int[,] flagarray)
    {
        int grid_end_x = flagarray.GetLength(0);
        int grid_end_y = flagarray.GetLength(1);
        node_grid_ = new Node[grid_end_x, grid_end_y];
        grid_size_x = grid_end_x;
        grid_size_y = grid_end_y;

        for (int y = 0; y < grid_end_y; y++)
        {
            for (int x = 0; x < grid_end_x; x++)
            {
                if (flagarray[x, y] == 1)
                {
                    node_grid_[x, y] = new Node(false, x, y);
                }
                else
                {
                    node_grid_[x, y] = new Node(true, x, y);
                }
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        // 3 by 3 grid search
        int node_grid_x;
        int node_grid_y;
        //for (int x = -1; x <= 1; x++)
        //{
        //    for(int y = -1; y <= 1; y++)
        //    {
        //        // if center cell skip, not neighbour
        //        if (x == 0 && y == 0)
        //        {
        //            continue;
        //        }
        //        // if cell is within grid
        //        node_grid_x = node.gridX_ + x;
        //        node_grid_y = node.gridY_ + y;
        //        if (node_grid_x >= 0 && node_grid_x < grid_size_x &&
        //            node_grid_y >= 0 && node_grid_y < grid_size_y)
        //        {
        //            neighbours.Add(node_grid_[node_grid_x, node_grid_y]);
        //        }
        //    }
        //}
        // nw neighbour
        node_grid_x = node.gridX_ + 1;
        node_grid_y = node.gridY_;
        if (node_grid_x >= 0 && node_grid_x < grid_size_x &&
            node_grid_y >= 0 && node_grid_y < grid_size_y)
        {
            neighbours.Add(node_grid_[node_grid_x, node_grid_y]);
        }
        // se neighbour
        node_grid_x = node.gridX_ - 1;
        node_grid_y = node.gridY_;
        if (node_grid_x >= 0 && node_grid_x < grid_size_x &&
            node_grid_y >= 0 && node_grid_y < grid_size_y)
        {
            neighbours.Add(node_grid_[node_grid_x, node_grid_y]);
        }
        // ne neighbour
        node_grid_x = node.gridX_;
        node_grid_y = node.gridY_ + 1;
        if (node_grid_x >= 0 && node_grid_x < grid_size_x &&
            node_grid_y >= 0 && node_grid_y < grid_size_y)
        {
            neighbours.Add(node_grid_[node_grid_x, node_grid_y]);
        }
        // sw neighbour
        node_grid_x = node.gridX_;
        node_grid_y = node.gridY_ - 1;
        if (node_grid_x >= 0 && node_grid_x < grid_size_x &&
            node_grid_y >= 0 && node_grid_y < grid_size_y)
        {
            neighbours.Add(node_grid_[node_grid_x, node_grid_y]);
        }
        return neighbours;
    }

    public Node GetNode(int x, int y)
    {
        return node_grid_[x, y];
    }
}
