using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AIModel;

public class DeckManager : MonoBehaviour
{
    public List<Sprite> deck;

    public List<Sprite> playerOneHand;
    public List<Sprite> playerTwoHand;
    public List<Sprite> groundCards;

    public Sprite back;

    public List<Button> playerOneCards;
    public List<Button> playerTwoCards;
    public Button groundCard;
    public Button drawedCard;
    public Button startButton;
    public Button deckButton;
    //public Button skrewButton;

    public Text playerOne;
    public Text playerTwo;

    private bool isMyTurn;
    private int displayed;
    private int round;
    private bool ready;
    private int i = 1;
    private string clickedButtonNameDraw;
    private string clickedButtonNameSwap;
    private string clickedButtonNameRid;

    private bool drawnedCard;
    private bool swapedCard;
    private bool chosenCard;
    private bool clickedCard;

    void Shuffle(List<Sprite> faces)
    {
        System.Random random = new System.Random();
        int n = faces.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            Sprite temp = faces[k];
            faces[k] = faces[n];
            faces[n] = temp;
        };
    }
    void CardsSetup()
    {
        DealCards(playerOneHand);
        DealCards(playerTwoHand);

        groundCards.Add(deck[0]);
        groundCard.image.sprite = deck[0];
        deck.RemoveAt(0);
        print(groundCards[0].name);
    }
    void DealCards(List<Sprite> hand)
    {
        foreach(var item in deck.Take(4))
        {
            hand.Add(item);
        }
        deck.RemoveRange(0, 4);
    }

    public void Start()
    {
        //Shuffle(deck);

        isMyTurn = true;
        displayed = 0;
        round = 1;
        ready = false;
        drawnedCard = false;
        swapedCard = false;
        chosenCard = false;
        clickedCard = false;
        clickedButtonNameDraw = "";
        clickedButtonNameSwap = "";
        clickedButtonNameRid = "";

        playerOneHand = new List<Sprite>();
        playerTwoHand = new List<Sprite>();
        groundCards = new List<Sprite>();

        deckButton.gameObject.SetActive(true);
        //skrewButton.gameObject.SetActive(false);
        drawedCard.gameObject.SetActive(false);

        CardsSetup();
        Round(round, isMyTurn);
    }
    private void Update()
    {
        Round(round, isMyTurn);
    }
    void Round(int r, bool t)
    {
        if (r == 6)
        {

        }
        else
        {
            if (t)
            {
                StartGame(playerOneHand);
                turn(t);
            }
            else
            {
                StartGame(playerTwoHand);
                turn(t);
            }
        }
    }
    public void EndRound()
    {
        round++;
    }
    void turn(bool turn)
    {
        if (turn)
        {
            playerOne.text = "YOU";
            playerTwo.text = "AI MODEL";
        }
        else
        {
            playerOne.text = "AI MODEL";
            playerTwo.text = "YOU";

            AIModel ai_model = new AIModel();
            ai_model.known_cards.Add(playerTwoHand[playerTwoHand.Count - 2]);
            ai_model.known_cards.Add(playerTwoHand[playerTwoHand.Count - 1]);

            node start = new node(groundCards[groundCards.Count - 1]);
            node end = new node("goal");

            Hashtable hashtable = new Hashtable
            {
                { start, (int) 0 },
                { end, (int) 0 },
            };


            node a = ai_model.Match(start, end, hashtable);
            node b = ai_model.Swap(start, end, hashtable);
            node c = ai_model.DrawSpecial(deck[0], start, end, hashtable);
            node d = ai_model.Draw(deck[0], start, end, hashtable);

            var solution = ai_model.Astar(start, end, hashtable);

            print(solution[1].state);
            print(solution[1].card.name);
            foreach (var card in solution[1].cards)
            {
                print(card.name);
            }


            if (ready)
            {
                groundCards[groundCards.Count - 1] = solution[1].card;
                groundCard.image.sprite = solution[1].card;

                playerTwoHand[playerTwoHand.Count - 2] = solution[1].cards[0];
                if (solution[1].cards.Count > 1)
                    playerTwoHand[playerTwoHand.Count - 1] = solution[1].cards[1];


                if (solution[1].state == "c" || solution[1].state == "d")
                {
                    deck.Remove(deck[0]);
                }

                if (solution[1].cards.Count < 2)
                {
                    playerTwoCards[playerTwoHand.IndexOf(solution[1].removed_card)].gameObject.SetActive(false);
                    playerTwoHand.Remove(solution[1].removed_card);
                }

                EndTurn();
                
            }


        }
    }

    public void EndTurn()
    {
        isMyTurn = !isMyTurn;
    }

    void StartGame(List<Sprite> _hand)
    {
        if (!ready)
        {
            playerOneCards[2].image.sprite = _hand[2];
            playerOneCards[3].image.sprite = _hand[3];
        }
        else
        {
            playerOneCards[2].image.sprite = back;
            playerOneCards[3].image.sprite = back;
        }
    }
    public void Ready()
    {
        ready = !ready;
        startButton.gameObject.SetActive(false);
        //skrewButton.gameObject.SetActive(true);
        deckButton.gameObject.SetActive(true);
    }
    public void ResetCards()
    {
        swapedCard = false;
        clickedCard = false;
        drawnedCard = false;
        clickedButtonNameSwap = "";
        clickedButtonNameRid = "";
        clickedButtonNameDraw = "";
    }
    public void FindName()
    {
        if (drawnedCard)
        {
            clickedButtonNameDraw = EventSystem.current.currentSelectedGameObject.name;
            SwapDrawedCard(clickedButtonNameDraw);
        }
        else if (swapedCard)
        {
            clickedButtonNameSwap = EventSystem.current.currentSelectedGameObject.name;
            SwapHandCard(clickedButtonNameSwap);
        }

    }
    public void FindRidName()
    {
        if (clickedCard)
        {
            GetRidCard(clickedButtonNameRid);
        }
    }
    public void DrawCard()
    {
        drawedCard.image.sprite = deck[0];
        deck.RemoveAt(0);
        drawedCard.gameObject.SetActive(true);
        drawnedCard = true;
    }
    public void SwapDrawedCard(string dname)
    {
        if (drawnedCard)
        {
            if (isMyTurn)
            {
                groundCard.image.sprite = playerOneHand[int.Parse(dname) - 1];
                groundCards.Add(playerOneHand[int.Parse(dname) - 1]);
                playerOneHand[int.Parse(dname) - 1] = drawedCard.image.sprite;

                drawedCard.gameObject.SetActive(false);
                ResetCards();
                EndTurn();
            }
            else
            {
                groundCard.image.sprite = playerTwoHand[int.Parse(dname) - 1];
                groundCards.Add(playerTwoHand[int.Parse(dname) - 1]);
                playerTwoHand[int.Parse(dname) - 1] = drawedCard.image.sprite;

                drawedCard.gameObject.SetActive(false);
                drawnedCard = false;
                ResetCards();
                EndTurn();
            }
        }
    }
    public void PutOnGround()
    {
        if (drawnedCard)
        {
            groundCard.image.sprite = drawedCard.image.sprite;
            groundCards.Add(drawedCard.image.sprite);
            drawedCard.gameObject.SetActive(false);
            //Special(drawedCard.gameObject.name);
            ResetCards();
            EndTurn();
        }
    }
    public void Special(string name)
    {
        switch (name)
        {
            case "7":
            case "8":

                break;
        }
    }
    public void SwapCard()
    {
        swapedCard = true;
    }
    public void SwapHandCard(string sname)
    {
        if (swapedCard)
        {
            if (isMyTurn)
            {
                Sprite temp = groundCard.image.sprite;
                groundCards.RemoveAt(groundCards.Count - 1);
                groundCard.image.sprite = playerOneHand[int.Parse(sname) - 1];
                groundCards.Add(playerOneHand[int.Parse(sname) - 1]);
                playerOneHand[int.Parse(sname) - 1] = temp;

                ResetCards();
                EndTurn();
            }
            else
            {
                Sprite temp = groundCard.image.sprite;
                groundCards.RemoveAt(groundCards.Count - 1);
                groundCard.image.sprite = playerTwoHand[int.Parse(sname) - 1];
                groundCards.Add(playerTwoHand[int.Parse(sname) - 1]);
                playerTwoHand[int.Parse(sname) - 1] = temp;

                ResetCards();
                EndTurn();
            }
        }
    }
    public void ClickedCard()
    {
        clickedCard = true;
        clickedButtonNameRid = EventSystem.current.currentSelectedGameObject.name;
    }
    public void GetRidCard(string rname)
    {
        if (isMyTurn)
        {
            if (groundCard.image.sprite == playerOneHand[int.Parse(rname) - 1])
            {
                groundCards.Add(playerOneHand[int.Parse(rname) - 1]);
                groundCard.image.sprite = playerOneHand[int.Parse(rname) - 1];
                playerOneHand.RemoveAt(int.Parse(rname) - 1);
                playerOneCards[int.Parse(rname) - 1].gameObject.SetActive(false);

                ResetCards();
                EndTurn();
            }
            else
            {
                playerOneHand.Add(groundCards[groundCards.Count - 1]);
                groundCards.RemoveAt(groundCards.Count - 1);
                if (groundCards.Count != 0)
                {
                    groundCard.image.sprite = groundCards[groundCards.Count - 1];
                }
                else
                {
                    groundCard.image.sprite = null;
                }
                playerOneCards[(int.Parse(rname) - 1) + 4].gameObject.SetActive(true);
                ResetCards();
                EndTurn();
            }
        }
        else
        {
            if (groundCard.image.sprite == playerTwoHand[int.Parse(rname) - 1])
            {
                groundCards.Add(playerTwoHand[int.Parse(rname) - 1]);
                groundCard.image.sprite = playerTwoHand[int.Parse(rname) - 1];
                playerTwoHand.RemoveAt(int.Parse(rname) - 1);
                playerTwoCards[int.Parse(rname) - 1].gameObject.SetActive(false);

                ResetCards();
                EndTurn();
            }
            else
            {
                playerTwoHand.Add(groundCards[groundCards.Count - 1]);
                groundCards.RemoveAt(groundCards.Count - 1);
                if (groundCards.Count != 0)
                {
                    groundCard.image.sprite = groundCards[groundCards.Count - 1];
                }
                else
                {
                    groundCard.image.sprite = null;
                }
                playerTwoCards[(int.Parse(rname) - 1) + 4].gameObject.SetActive(true);
                ResetCards();
                EndTurn();
            }
        }
    }
}
