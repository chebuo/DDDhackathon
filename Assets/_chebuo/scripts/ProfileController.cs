using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class ProfileController : MonoBehaviour
{
    [SerializeField]GameObject profilePanel;
    [SerializeField]InputField nameInput;
    [SerializeField]GameSelectData gameSelectData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn)await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    void Update()
    {
        
    }

    public async void ChangeName()
    {
        if(string.IsNullOrEmpty(nameInput.text))return;
        PlayerPrefs.SetString("playerName", nameInput.text);
        gameSelectData.playerName = nameInput.text;
        await AuthenticationService.Instance.UpdatePlayerNameAsync(nameInput.text);
    }

    public void OpenPanel()
    {
        profilePanel.gameObject.SetActive(true);
    }

    public void CancelPanel()
    {
        profilePanel.gameObject.SetActive(false);
    }
}
