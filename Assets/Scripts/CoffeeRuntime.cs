using UnityEngine;

public class CoffeeRuntime : MonoBehaviour
{
    public static CoffeeRuntime Instance;

    // The active recipe data (set by Book Manager)
    public CoffeeBeanRecipe activeRecipe;

    //Player perfomance values
    public float playerFinalWeight;
    public float playerGrindSize;
    public float playerWaterTemp;
    public float playerBrewTime;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(gameObject);
        }
    } 
}
