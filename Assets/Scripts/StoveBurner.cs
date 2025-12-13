using UnityEngine;

public class StoveBurner : MonoBehaviour
{
    public TemperatureUI tempUI;
    public GameObject fireArt;
    public Transform dropPoint;

    public bool hasKettle = false;
    private bool burnerOn = false;

    public AudioSource FireAudio;

    public void SetKettlePresent(bool value)
    {
        hasKettle = value;

        if (value)
        {
            // Show UI immediately
            tempUI.ShowUI();

            // If burner was already on, start heating
            if (burnerOn)
                tempUI.RequestHeating();
        }
        else
        {

            // Kettle removed → stop burner & fire
            burnerOn = false;
            fireArt.SetActive(false);

            if (FireAudio != null && FireAudio.isPlaying) {
                FireAudio.Stop();

                // Kettle removed → freeze temp
                tempUI.StopHeating();
                tempUI.HideUI();
            }
        }
    }
    public void SetBurnerState(bool on)
    {
        burnerOn = on;
        fireArt.SetActive(on);

        // 🔥 Fire audio
        if (FireAudio != null)
        {
            if (on)
            {
                if (!FireAudio.isPlaying)
                    FireAudio.Play();
            }
            else
            {
                FireAudio.Stop();
            }
        }

        if (on && hasKettle)
        {
            tempUI.RequestHeating();
        }
        else
        {
            // 🔒 Burner OFF → freeze temperature
            tempUI.StopHeating();
        }
    }

}

