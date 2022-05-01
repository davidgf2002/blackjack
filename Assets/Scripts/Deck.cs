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
        //Mostrar dinero
        moneyText.text = money.ToString();

        //Ajustar el valor máximo del slider con el dinero
        slider.maxValue = money;

        //Slider obtener valor
        slider.onValueChanged.AddListener((v) =>
        {
            apuesta = Convert.ToInt32(v);
            sliderText.text = v.ToString("0");
        });

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

        }
        for (int i = 0; i < 2; i++)
        {
            PushDealer();
        }

        //Sumatorio de los valores
        sumatorioPlayer = values[0] + values[1];
        sumatorioDealer = values[2] + values[3];
        CalculateProbabilities();
        //Mostrar por pantalla en valor del jugador
        numCartas.text = sumatorioPlayer.ToString();

        //Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje 
        if (sumatorioPlayer == 21)
        {
            //Se acaba el juego y se muestra la pantalla de victoria
            GameOver("Victoria");

            //Se ocultan los botones
            EstadoBotones(false);
        }
        else if (sumatorioDealer == 21)
        {
            //Se muestra la carta del dealer que esta escondida
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

            //Se acaba el juego y se muestra la pantalla de derrota
            GameOver("Derrota");

            //Se ocultan los botones
            EstadoBotones(false);
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
        //------------------
        //Mayor que 21
        int sumatorio = sumatorioPlayer;

        if ((sumatorio > 11) && (sumatorio < 22))
        {
            int resPlayer = 21 - sumatorio;
            int res = 13 - resPlayer;
            float div = (float)((float)res / 13.0) * 100;
            probMessage.text = div.ToString("0.00");
        }
        else
        {
            probMessage.text = "0";
        }

        //Entre 17 y 21
        if ((sumatorio < 7) || (sumatorio > 20))
        {
            probMessage17.text = "0";
        }
        if ((sumatorio > 6) && (sumatorio < 12))
        {
            float num = (float)(sumatorio - 3.0);
            float res = (num / 13) * 100;
            probMessage17.text = res.ToString("0.00");
        }
        if ((sumatorio >= 12) && (sumatorio < 17))
        {
            float num = (float)((5.0 / 13.0) * 100.0);
            probMessage17.text = num.ToString("0.00");
        }
        if ((sumatorio >= 17) && (sumatorio < 21))
        {
            float num = (float)(21.0 - sumatorio);
            float res = (float)((num / 13.0) * 100.0);
            probMessage17.text = res.ToString("0.00");
        }
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
        //Se muestra la carta oculta del dealer
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

        //Repartimos carta al jugador
        PushPlayer();

        //Calculamos los valores y lo mostramos por pantalla
        sumatorioPlayer = sumatorioPlayer + values[playerIndex - 1];
        numCartas.text = sumatorioPlayer.ToString();
        numCartasDealer.text = sumatorioDealer.ToString();

        CalculateProbabilities();

        //Comprobamos si el jugador ya ha perdido y mostramos mensaje
        if (sumatorioPlayer > 21)
        {
            //Se acaba el juego y se muestra la pantalla de derrota
            GameOver("Derrota");

            //Se muestra el valor de Dealer
            numCartasDealer.text = sumatorioDealer.ToString();

            //Se ocultan los botones
            EstadoBotones(false);

            //Pierdes el dinero de la apuesta
            Economia(false, apuesta);
        }
    }

    public void Stand()
    {
        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */

        //Se muestra la carta oculta del dealer
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

        //Se ocultan los botones
        EstadoBotones(false);

        numCartasDealer.text = sumatorioDealer.ToString();

        while (sumatorioDealer < 17)
        {
            PushDealer();
            sumatorioDealer += values[playerIndex - 1];
            numCartasDealer.text = sumatorioDealer.ToString();


        }

        if ((sumatorioDealer == 21) && (sumatorioPlayer == 21) || (sumatorioDealer == sumatorioPlayer) && (sumatorioDealer < 22) && (sumatorioPlayer < 22))
        {
            //Se acaba el juego y se muestra la pantalla de Empate
            GameOver("Empate");
        }
        if (sumatorioPlayer == 21)
        {
            //Se acaba el juego y se muestra la pantalla de Victoria
            GameOver("Victoria");

            //Ganas el dinero de la apuesta por 2
            Economia(true, apuesta);
        }
        if ((sumatorioDealer > sumatorioPlayer) && (sumatorioDealer < 22))
        {
            //Se acaba el juego y se muestra la pantalla de derrota
            GameOver("Derrota");

            //Pierdes el dinero de la apuesta
            Economia(false, apuesta);
        }
        if ((sumatorioPlayer > sumatorioDealer) && (sumatorioPlayer < 22) || (sumatorioPlayer < sumatorioDealer) && (sumatorioDealer > 21))
        {
            //Se acaba el juego y se muestra la pantalla de Victoria
            GameOver("Victoria");

            //Ganas el dinero de la apuesta por 2
            Economia(true, apuesta);
        }
        if ((sumatorioPlayer > 21) && (sumatorioDealer > 21))
        {
            //Se acaba el juego y se muestra la pantalla de derrota
            GameOver("Ambos Pierden");

            //Pierdes el dinero de la apuesta
            Economia(false, apuesta);
        }
    }

    public void PlayAgain()
    {
        //Se muestran los botones
        EstadoBotones(true);

        finalMessage.text = "";
        letrero.SetActive(false);

        //Se borran las carta
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();

        //Se reinician los contadores
        playerIndex = 0;
        sumatorioPlayer = 0;

        //Comienza el juego
        slider.maxValue = money;
        numCartasDealer.text = "0";
        ShuffleCards();
        StartGame();
    }

    private void EstadoBotones(bool estado)
    {
        //Muestran u ocultan los botones
        hitButton.interactable = estado;
        stickButton.interactable = estado;
    }

    private void GameOver(string text)
    {
        finalMessage.text = text;
        letrero.SetActive(true);
    }

    private void Economia(bool estado, int dinero)
    {
        if (estado)
        {
            money = money + (dinero * 2);
            moneyText.text = money.ToString();
        }
        else
        {
            money = money - apuesta;
            moneyText.text = money.ToString();
        }
    }
}
