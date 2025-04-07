using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public enum HealthBarMechanics
{
    Classic,
    LivesSystem,
    OneHitKill,
}




public class HealthController : MonoBehaviour
{

    [Header("Visuals")]
    public GameObject Fill;
    public GameObject Heart;
    public Color High;
    public Color Low;


    [Header("Settings")]
    public HealthBarMechanics healthBarMechanics;
    public GameObject player;
    public float MaxHealth = 100;
    public float Health = 0f;

    [Header("regen settings")]
    public float RegenAmount;
    public float setRegenTime;
    public float RegenTime;
    public bool autoRegen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = MaxHealth; 
    }

    // Update is called once per frame
    void Update()
    {
        
        float normalizedHealth = Health / MaxHealth;
        
        bar_visuals(normalizedHealth);
        AutoRegen();

        if(player && Health <= 0) Destroy(player);       
    } 

    void AutoRegen()
    {
         if(autoRegen)
        {
            if(RegenTime >= 0 && Health < MaxHealth) RegenTime -= Time.deltaTime;
            if(RegenTime <= 0 && Health <= MaxHealth) StartCoroutine(fillRegeningIEnumerator());
        }

        if(Health > MaxHealth)
        {
            Health = MaxHealth;
            RegenTime = setRegenTime;
        }
    }

    void bar_visuals(float normalizedHealth)
    {
        RectTransform Bar = Fill.GetComponent<RectTransform>();
        float width = Mathf.Lerp(52f, 0f, normalizedHealth);
        Bar.offsetMax = new Vector2(-width, 0);
        Image BarIm = Fill.GetComponent<Image>();
        BarIm.color = Color.Lerp(Low, High, normalizedHealth);
    }

    IEnumerator fillRegeningIEnumerator()
    {
        Health += RegenAmount;
        RegenTime = setRegenTime;
        yield return new WaitForSeconds(2);
    }

}
