using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;


public class FormHandler : MonoBehaviour
{
    #region Variaveis
    private string fullNameString, firstNameString, emailString;
    private string yearString = string.Empty;
    private string monthString = string.Empty;
    private string dayString = string.Empty;
    private string bornString = string.Empty;

    private string[] splitName;

    public InputField nameInput,emailInput, dayInput, monthInput, yearInput;

    public Text returnField, emailWarning, birthdateWarning , nameWarning;

    private string URL;

    public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
    #endregion

    void Start()
    {
        Screen.SetResolution(1080, 1920, true);
        FormatURL();
    }

    #region Funcoes de leitura dos Inputs
    // Salva o que foi digitado nos inputs
    public void ReadNameInput()
    {

        fullNameString = nameInput.text;
        splitName = fullNameString.Split(" " [0]);
        firstNameString = splitName[0].ToLower();
        nameWarning.text = string.Empty;

        if (nameInput.text == string.Empty)
        {
            nameWarning.text = "Sem nome";
            nameWarning.color = Color.red;
            fullNameString = string.Empty;
        }

    }

    public void ReadEmailInput()
    {
        emailString = emailInput.text;
        var isValidEmail = validateEmail(emailString);

        if (isValidEmail)
        {
            emailWarning.text = "";
        }
        else
        {
            emailInput.text = string.Empty;
            emailString = string.Empty;
            emailInput.Select();
            emailWarning.text = "* Verifique se seu e-mail esta correto.";
            emailWarning.color = Color.red;
        }
    }

    public void ReadDayInput()
    {

        dayString = dayInput.text;
        int day;
        int.TryParse(dayInput.text, out day);


        if (day > 30 || day < 0)
        {
            birthdateWarning.text = "Dia invalido!";
            birthdateWarning.color = Color.red;
            dayString = string.Empty;
            dayInput.text = string.Empty;
            return;
        }
        ConcatanateBirthdate();
    }

    public void ReadMonthInput ()
    {
        monthString = monthInput.text;
        int month;
        int.TryParse(monthInput.text, out month);

        if (month > 12 || month < 0)
        {
            birthdateWarning.text = "Mes invalido!";
            birthdateWarning.color = Color.red;
            monthString = string.Empty;
            monthInput.text = string.Empty;
            return;
        }
        ConcatanateBirthdate();
    }

    public void ReadYearInput()
    {
        yearString = yearInput.text;
        int year;
        int.TryParse(yearInput.text, out year);

        if (year > 2021 || year < 1910)
        {
            birthdateWarning.text = "Ano invalido!";
            birthdateWarning.color = Color.red;
            yearString = string.Empty;
            yearInput.text = string.Empty;
            return;
        }
        ConcatanateBirthdate();
    }

    #endregion

    #region Manipulacao de strings
    //checa se os campos de data foram digitados e concatena uma string no formato DD-MM-AAAA
    private void ConcatanateBirthdate()
    {
        if (dayString != string.Empty && monthString != string.Empty && yearString != string.Empty)
        {
            bornString = dayString + "-" + monthString + "-" + yearString;
            birthdateWarning.text = "";

        }

    }

    //monta a URL com as variaveis dos inputs
    public void FormatURL()
    {
        URL = string.Format
            ("https://sweetbonus.com.br/sweet-juice/trainee-test/submit?candidate={0}&fullname={1}&email={2}&birthdate={3}",
            firstNameString, fullNameString, emailString, bornString);
    }
    #endregion

    #region Leitura do json
    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            returnField.text = uwr.error;
            Debug.Log("Erro: " + uwr.error);
        }
        else
        {
            returnField.text = uwr.downloadHandler.text;
            Debug.Log("Recebido: " + uwr.downloadHandler.text);
        }
        jsonDataClass jsonData = JsonUtility.FromJson<jsonDataClass>(uwr.downloadHandler.text);


        DataProcess(jsonData);
    }

    private void DataProcess(jsonDataClass jsonData)
    {
        switch (jsonData.success)
        {
            case false:
                if (jsonData.status == "empty_candidate" || jsonData.status == "empty_fullname")
                {
                    returnField.text = "Digite seu nome!";
                }

                if (jsonData.status == "empty_email")
                {
                    returnField.text = "Digite seu email!";
                }

                if (jsonData.status == "empty_birthdate")
                {
                    returnField.text = "Esqueceu quando nasceu?";
                }
                break;
            default:
                returnField.text = "Muito Obrigado! " + jsonData.data.message;
                break;
        }
    }
    #endregion

    //booleana para validar o email digitado
    public static bool validateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, MatchEmailPattern);
        else
            return false;
    }

    public void OnButtonSend() => StartCoroutine(GetRequest(URL));
}
