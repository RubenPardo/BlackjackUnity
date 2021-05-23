using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;


    public int[] values = new int[52];
    int cardIndex = 0;    
       
    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int contCardValues = 0;
        for(int i=1;i<=4; i++)
        {
            for (int j = 1; j <= 13; j++)
            {
                if (j > 10)
                {
                    values[contCardValues++] = 10;
                }
                else
                {
                    values[contCardValues++] = j;
                }
            }
        }


    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        // crear un array de indices, randomizarlo 
        int[] indices = Enumerable.Range(0, values.Length).ToArray();
        System.Random rnd = new System.Random();
        indices = indices.OrderBy(x => rnd.Next()).ToArray();

    
        // poner los dos arrays en el orden de los indices
        int[] tempValues = new int[52];
        Sprite[] tempFaces = new Sprite[52];
        // arr[i] should be present at index[i] index
        for (int i = 0; i < 52; i++)
        {
            tempValues[indices[i]] = values[i];
            tempFaces[indices[i]] = faces[i];
        }
        // Copy temp[] to arr[]
        for (int i = 0; i < 52; i++)
        {
            values[i] = tempValues[i];
            faces[i] = tempFaces[i];
            indices[i] = i;
        }
        


    }

    void StartGame()
    {

        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            voltearCartaDealer(false);
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            if (comprobarBlackJackJugador())
            {
                endGame(true);

            }else if (comprobarBlackJackDealer())
            {
                endGame(false);
            }
        }
    }

    private void voltearCartaDealer(bool v)
    {
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(v);
    }

    private bool comprobarBlackJackJugador()
    {
        
        if (player.GetComponent<CardHand>().points == 21)
        {
            return true;
        }
        else
        {
            // si hay un 1 comprobar si con el 11 hace 21
            foreach(GameObject carta in player.GetComponent<CardHand>().cards)
            {
                if(carta.GetComponent<CardModel>().value == 1)
                {
                    if(((player.GetComponent<CardHand>().points-1) + 11 )== 21)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool comprobarBlackJackDealer()
    {
        if (dealer.GetComponent<CardHand>().points == 21)
        {
            return true;
        }
        else
        {
            // si hay un 1 comprobar si con el 11 hace 21
            foreach (GameObject carta in dealer.GetComponent<CardHand>().cards)
            {
                if (carta.GetComponent<CardModel>().value == 1)
                {
                    if (((dealer.GetComponent<CardHand>().points - 1) + 11) == 21)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
        CardHand playerHand = player.GetComponent<CardHand>();
        CardHand dealerHand = dealer.GetComponent<CardHand>();

        List<CardModel> cardsJugador = playerHand.cards
            .Select(card => card.GetComponent<CardModel>()).ToList();
        List<CardModel> dealerCards = dealerHand.cards
            .Select(card => card.GetComponent<CardModel>()).ToList();

        // Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador


        //Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
        // casos favorables = todas las sumas que esten entre 17 y 21 
        // casos posibles = cartas restantes
        int puntosJugador = playerHand.points;
        float contSumasEntre17_21 =0.0f;// casos favorables
        float casosPosibles = values.Length - cardIndex + 1.0f;// casos posibles, cartas restantes mas 1 por cada As
        int suma = 0;

        // contar cuantos As hay, tiene 2 sumas posibles
        for (int i = cardIndex; i < values.Length; i++)
        {
            if(values[i] == 1)
            {
                casosPosibles+=1.0f;
            }
        }
        // contar casos favorables
        for (int i=cardIndex; i < values.Length; i++)
        {
            suma = puntosJugador + values[i];
            if(suma >= 17 && suma <= 21)
            {
               
                contSumasEntre17_21++;
            }

            // si el valor 1 contemplar el 11
            if(values[i] == 1)
            {
                suma = puntosJugador + 11;
                if (suma >= 17 && suma <= 21)
                {
                    contSumasEntre17_21++;
                }
            }
        }

        Debug.Log("------------ENTRE 17 Y 21");
        Debug.Log("Casos posibles: " + casosPosibles);
        Debug.Log("Casos favorables: " + contSumasEntre17_21);
        double prob = (contSumasEntre17_21/casosPosibles)*100;
        string prob17_21 = "Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta: " + prob;
        probMessage.text = prob17_21;


        //Probabilidad de que el jugador obtenga más de 21 si pide una carta
        int casosFavMas21 = 0;
        // casos favorables todas aquellas sumas que sobre pasen el 21
        for (int i = cardIndex; i < values.Length; i++)
        {
            suma = puntosJugador + values[i];
            if (suma > 21)
            {

                casosFavMas21++;
            }

            // si el valor 1 contemplar el 11
            if (values[i] == 1)
            {
                suma = puntosJugador + 11;
                if (suma >21)
                {
                    casosFavMas21++;
                }
            }
        }

        Debug.Log("------------ MAS DE 21");
        Debug.Log("Casos posibles: " + casosPosibles);
        Debug.Log("Casos favorables: " + casosFavMas21);
        double prob2 = (casosFavMas21 / casosPosibles) * 100;
        string probMas21 = "\nProbabilidad de que el jugador obtenga mas de 21 si pide carta: " + prob2;
        probMessage.text += probMas21;

    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        
        
        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (player.GetComponent<CardHand>().points > 21)
        {
            endGame(false);

        }

        if (comprobarBlackJackJugador())
        {
            endGame(true);
        }

    }

    private void endGame(bool victoria)
    {
        voltearCartaDealer(true);
        finalMessage.text = victoria ? "Has ganado" : "Has perdido" ;
      
        hitButton.interactable = false;
        stickButton.interactable = false;
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        
        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */

        Debug.Log("STAND--------------------: " + dealer.GetComponent<CardHand>().points);
        while (dealer.GetComponent<CardHand>().points < 17)
        {
            PushDealer();

            Debug.Log("Puntos dealer: " + dealer.GetComponent<CardHand>().points);
        }

        // si tienes mas puntos que el ganas
        if(dealer.GetComponent<CardHand>().points < player.GetComponent<CardHand>().points)
        {
            endGame(true);
        }
        else if (dealer.GetComponent<CardHand>().points == player.GetComponent<CardHand>().points)
        {
            // empate
            empate();
        }
        else
        {
            // si obtienes menos pierdes, pero si se pasa de 21 ganas
            if(dealer.GetComponent<CardHand>().points < 21)
            {
                endGame(false);
            }
            else
            {
                endGame(true);
            }
        }
         
    }

    private void empate()
    {
        voltearCartaDealer(true);
        finalMessage.text = "Empate";
        hitButton.interactable = false;
        stickButton.interactable = false;
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
}
