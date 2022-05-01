using UnityEngine;
using UnityEngine.UI;
using System;

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
    public Text probMessage17;
    public Text numCartas;
    public Text numCartasDealer;
    public GameObject letrero;
    public int[] values = new int[52];
    int playerIndex = 0;
    int sumatorioPlayer = 0;
    int sumatorioDealer = 0;

    //--------------------- Banca
    public Text moneyText;
    int money = 1000;
    int apuesta;
    public Slider slider;
    public Text sliderText;

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

        int num = 0;
        for (int i = 0; i < faces.Length; i++)
        {
            num = i + 1;
            num %= 13;

            if ((num > 10) || (num == 0))
            {
                num = 10;
            }
            values[i] = num++;
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        for (int i = faces.Length - 1; i >= 0; i--)
        {
            int j = UnityEngine.Random.Range(0, 52);
            Sprite face = faces[i];
            faces[i] = faces[j];
            faces[j] = face;

            int value = values[i];
            values[i] = values[j];
            values[j] = value;
        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta (X)  
         */

    }

    void PushDealer()
    {
        //Se implemente ShuffleCards y se pone una carta en la mesa
        dealer.GetComponent<CardHand>().Push(faces[playerIndex], values[playerIndex]);
        playerIndex++;
    }

    void PushPlayer()
    {
        //Se implemente ShuffleCards y se pone una carta en la mesa
        player.GetComponent<CardHand>().Push(faces[playerIndex], values[playerIndex]);
        playerIndex++;
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
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        playerIndex = 0;
        ShuffleCards();
        StartGame();
    }
}
