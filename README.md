# LightWeightSignalBuses
These are just a couple of scripts used mainly to use as messaging system, these systems uses types and the variables wrapped on these types
this messaging system has been inspired by Zenject SignalBus,and also supports signal interfaces in the same way that the "AbstractSignals" feature i added in Zenject/Extenject (see mentioned pull request about Abstract Signals here https://github.com/modesttree/Zenject/pull/91)

Also comes with a little CompositeDisposable implementation inspired from UniRx/CysharpUnitask.

fast integration and easy to use, perfect for light projects.



example for LightWeightSignalBus usage
```csharp
public class Player : MonoBehaviour
{
    float hp;
  
    void OnPayerDamaged(float damage)
    {
        hp-=damage;
    
        //We fire a signal in this way
        SignalBus<SignalPlayerDamaged>.Fire(new SignalPlayerDamaged{playerHp = hp});
    }
    
    void OnPayerHealed(float heal)
    {
        hp+=heal;
        SignalBus<SignalPlayerHealed>.Fire(new SignalPlayerHealed{playerHp = hp});
    }
}

public class HpBar : MonoBehaviour
{
    //Needed to dispose the signals when HpBar is destroyed
    CompositeDisposable disposables = new CompositeDisposable();

    void Start()
    {
        //This register the signal so every time each signal is fired it calls ShowHP with its data
        SignalBus<SignalPlayerDamaged>.Subscribe(x=> ShowHP(x.playerHp)).AddTo(disposables);
        SignalBus<SignalPlayerHealed>.Subscribe(x=> ShowHP(x.playerHp)).AddTo(disposables);
    }
  
    void ShowHp(float hpToShow)
    {
        //Do something to show the hpToShowValue in a ui bar or something
    }
    
    void OnDestroy()
    {
        //Ensures que signal is not called anymore when  player gets damage, when the HpBar is destroyed
        //This is important because if you dont dispose it, after destroying the HpBar, the signal will try to call ShowHP of the Destroyed HpBar
        //So BOOOM! Errors if you dont do it!
        disposables.Dispose();
    }
}

//The signal struct
public struct SignalPlayerDamaged
{
    public float playerHp;
}
public struct SignalPlayerHealed
{
    public float playerHp;
}
```


example for InterfaceCompatibleSignalBus usage
```csharp
public class Player : MonoBehaviour
{
    float hp;
  
    void OnPayerDamaged(float damage)
    {
        hp-=damage;
    
        //We fire a signal with their interfaces in this way
        SignalBus.AbstractFire<SignalPlayerDamaged>(new SignalPlayerDamaged{playerHp = hp});
    }
    
    void OnPayerHealed(float heal)
    {
        hp+=heal;
        //Different signal but with the same interface implementation
        SignalBus.AbstractFire<SignalPlayerHealed>(new SignalPlayerHealed{playerHp = hp});
    }
}

public class HpBar : MonoBehaviour
{
    void Start()
    {
        //This register the signal so every time this signal is fired it calls ShowHP with its data;
        SignalBus<ISignalPlayerHpChanged>.Subscribe(x=> ShowHP(x.playerHp))
    }
  
    void ShowHp(float hpToShow)
    {
        //Do something to show the hpToShowValue in a ui bar or something
    }
}

//The signal types
public struct SignalPlayerDamaged : ISignalPlayerHpChanged
{
    float ISignalPlayerHpChanged.PlayerHp => playerHp;
    public float playerHp;
}
public struct SignalPlayerHealed : ISignalPlayerHpChanged
{
    float ISignalPlayerHpChanged.PlayerHp => playerHp;
    public float playerHp;
}

//The interface for the signals
public interface ISignalPlayerHpChanged
{
    float PlayerHp { get; }
}
```
