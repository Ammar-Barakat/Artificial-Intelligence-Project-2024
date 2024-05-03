using LSCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AIModel
{
    public List<Sprite> known_cards = new List<Sprite>();
    public class node
    {
        public string state { get; set; }
        public double cost { get; set; }
        public double fn { get; set; }
        public List<node> neighbours { get; set; } = new List<node>();


        public List<Sprite> cards { get; set; } = new List<Sprite>();
        public Sprite? card { get; set; }
        public Sprite? removed_card { get; set; }

        public node(string state)
        {
            this.state = state;
        }
        public node(Sprite sprite)
        {
            this.state = sprite.name;
            this.card = sprite;
        }
        public void AddNeighbour(node node, double cost)
        {
            this.neighbours.Add(node);
            node.cost = cost;
        }
    }

    public List<node> Astar(node start, node end, Hashtable hashtable)
    {
        List<node> solution = new List<node>();

        PriorityQueue pq = new PriorityQueue();
        pq.Enqueue(start);

        while (pq.Count != 0)
        {
            node current = (node)pq.Dequeue();

            solution.Add(current);

            if (current.state == end.state)
            {
                return solution;
            }

            foreach (var item in current.neighbours)
            {
                item.cost = current.cost + item.cost;
                item.fn = item.cost + (int)hashtable[item];
                pq.Enqueue(item);

            }
        }
        return null;
    }

    public node Match(node start, node end, Hashtable hashtable)
    {
        List<Sprite> match_known_cards = new List<Sprite>();
        foreach (var item in known_cards)
        {
            match_known_cards.Add(item);
        }

        node a = new node("a");

        int sum = 0;

        if (match_known_cards.Contains(start.card))
        {
            //a.state = start.state;

            foreach(var item in match_known_cards)
            {
                sum += int.Parse(item.name.Length > 2 ? item.name.Substring(0, 2) : item.name);
            }

            start.AddNeighbour(a, sum);

            match_known_cards.Remove(start.card);
            a.removed_card = start.card;

            sum = 0;
            foreach (var item in match_known_cards)
            {
                sum += int.Parse(item.name.Length > 2 ? item.name.Substring(0, 2) : item.name);
            }

            a.AddNeighbour(end, int.Parse(start.state.Length > 2 ? start.state.Substring(0, 2) : start.state));
            hashtable.Add(a, sum);

            a.card = start.card;
            a.cards = match_known_cards;
        }
        else
        {
            //a.state = "99";

            sum = 0;
            foreach (var item in match_known_cards)
            {
                sum += int.Parse(item.name.Length > 2 ? item.name.Substring(0, 2) : item.name);
            }

            start.AddNeighbour(a, sum);
            a.AddNeighbour(end, 99);
            hashtable.Add(a, sum);

            a.card = null;
            a.cards = match_known_cards;
        }

        return a;
    }
    public node Swap(node start, node end, Hashtable hashtable)
    {
        List<Sprite> swap_known_cards = new List<Sprite>();
        foreach(var item in known_cards)
        {
            swap_known_cards.Add(item);
        }

        node b = new node("b");

        int sum = 0;

        List<int> new_swap_known_cards = new List<int>();
        foreach(var item in swap_known_cards) 
        {
            new_swap_known_cards.Add(int.Parse(item.name.Length > 2 ? item.name.Substring (0, 2) : item.name));
        }

        if(int.Parse(start.state.Length > 2 ? start.state.Substring(0, 2) : start.state) < new_swap_known_cards.Max())
        {
            foreach(var item in new_swap_known_cards)
            {
                sum += item;
            }

            var temp = start.card;
            b.card = swap_known_cards[new_swap_known_cards.IndexOf(new_swap_known_cards.Max())];

            var i = swap_known_cards[new_swap_known_cards.IndexOf(new_swap_known_cards.Max())];

            b.AddNeighbour(end, new_swap_known_cards.Max());

            new_swap_known_cards.Remove(new_swap_known_cards.Max());
            new_swap_known_cards.Add(int.Parse(temp.name.Length > 2 ? temp.name.Substring(0,2) : temp.name));

            swap_known_cards.Remove(i);
            swap_known_cards.Add(temp);
            b.cards = swap_known_cards;


            start.AddNeighbour(b, sum);

            sum = 0;
            foreach (var item in new_swap_known_cards)
            {
                sum += item;
            }
            hashtable.Add(b, sum);
        }
        else
        {
            sum = 0;
            foreach (var item in swap_known_cards)
            {
                sum += int.Parse(item.name.Length > 2 ? item.name.Substring(0, 2) : item.name);
            }

            start.AddNeighbour(b, sum);
            b.AddNeighbour(end, 99);
            hashtable.Add(b, sum);

            b.card = null;
            b.cards = swap_known_cards;
        }

        return b;
    }
    public node DrawSpecial(Sprite drawed_card, node start, node end, Hashtable hashtable)
    {
        List<Sprite> draw_special_known_cards = new List<Sprite>();
        foreach (var item in known_cards)
        {
            draw_special_known_cards.Add(item);
        }

        int sum = 0;

        node c = new node("c");

        if(drawed_card.name.Length > 2)
        {
            //c.state = drawed_card;

            foreach(var item in draw_special_known_cards)
            {
                sum += int.Parse(item.name.Length > 2 ? item.name.Substring(0, 2) : item.name);
            }

            start.AddNeighbour(c, sum);
            c.AddNeighbour(end, int.Parse(drawed_card.name.Length > 2 ? drawed_card.name.Substring(0, 2) : drawed_card.name));
            hashtable.Add(c, sum);

            c.card = drawed_card;
            c.cards = draw_special_known_cards;
        }
        else
        {
            //c.state = "99";

            sum = 0;
            foreach (var item in draw_special_known_cards)
            {
                sum += int.Parse(item.name.Length > 2 ? item.name.Substring(0, 2) : item.name);
            }

            start.AddNeighbour(c, sum);
            c.AddNeighbour(end, 99);
            hashtable.Add(c, sum);

            c.card = null;
            c.cards = draw_special_known_cards;
        }

        return c;
    }
    public node Draw(Sprite drawed_card, node start, node end, Hashtable hashtable)
    {
        List<Sprite> draw_known_cards = new List<Sprite>();
        foreach (var item in known_cards)
        {
            draw_known_cards.Add(item);
        }

        int sum = 0;

        node d = new node("d");

        List<int> new_draw_known_cards = new List<int>();
        foreach (var item in draw_known_cards)
        {
            new_draw_known_cards.Add(int.Parse(item.name.Length > 2 ? item.name.Substring(0, 2) : item.name));
        }

        if (int.Parse(drawed_card.name.Length > 2 ? drawed_card.name.Substring(0, 2) : drawed_card.name) < new_draw_known_cards.Max())
        {
            foreach (var item in new_draw_known_cards)
            {
                sum += item;
            }

            var temp = drawed_card;
            d.card = draw_known_cards[new_draw_known_cards.IndexOf(new_draw_known_cards.Max())];

            var i = draw_known_cards[new_draw_known_cards.IndexOf(new_draw_known_cards.Max())];


            d.AddNeighbour(end, new_draw_known_cards.Max());

            new_draw_known_cards.Remove(new_draw_known_cards.Max());
            new_draw_known_cards.Add(int.Parse(temp.name.Length > 2 ? temp.name.Substring(0, 2) : temp.name));

            draw_known_cards.Remove(i);
            draw_known_cards.Add(temp);
            d.cards = draw_known_cards;


            start.AddNeighbour(d, sum);

            sum = 0;
            foreach (var item in new_draw_known_cards)
            {
                sum += item;
            }
            hashtable.Add(d, sum);

        }
        else
        {
            sum = 0;
            foreach (var item in draw_known_cards)
            {
                sum += int.Parse(item.name.Length > 2 ? item.name.Substring(0, 2) : item.name);
            }

            start.AddNeighbour(d, sum);
            d.AddNeighbour(end, int.Parse(drawed_card.name.Length > 2 ? drawed_card.name.Substring(0, 2) : drawed_card.name));
            hashtable.Add(d, sum);

            d.card = drawed_card;
            d.cards = draw_known_cards;
        }

        return d;
    }
}
