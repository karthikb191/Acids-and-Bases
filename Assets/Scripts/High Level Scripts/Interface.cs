using UnityEngine;
using UnityEngine.UI;

public interface ICharacter
{
    //void UpdateState(States s);
    //UserInput GetInput();
    //void Move();
    //void PlayAnimation();
    //void PlayAudio();   
}

public interface IPlayer
{
    //Inventory will be a public variable of the player
    void UseSkill();
    void GetActiveItem();
    void SetActiveItem(IItem item);
    void UseItem(IItem item);
    void ThrowItem(IItem item);
    void DropItem(IItem item);
    void PickUpItem(IItem item);                //This is called by the item the player is picking up
}

public interface IEnemy
{
    //UserInput GetInput(AI aiComponent);
    Vector3 GoBackToOriginalPosition(Node homeNode);
    Node ResetHomePosition();    //This returns the new home node
    void RoamAround();
    void ChasePlayer(bool playerFound);
    void UpdateChase(bool chasing);
    void UseSkill(bool useSkill);
    void Search(float coolDown, bool searching);
    void GiveUpChase();
}

abstract class EnemyBase : Character
{
    protected abstract Vector3 GoBackToOriginalPosition(Node homeNode);
    protected abstract Node ResetHomePosition();    //This returns the new home node
    protected abstract void RoamAround();
    protected abstract void ChasePlayer(bool playerFound);
    protected abstract void UpdateChase(bool chasing);
    protected abstract void UseSkill(bool useSkill);
    protected abstract void Search(float coolDown, bool searching);
    protected abstract void GiveUpChase();
}


//Inventory information is stored on the character
public interface IInventory
{
    IItem SetActiveItem(IPlayer p);         //Called when user interacts with the inventory canvas
    IItem GetActiveItem();                  //Called by the player, which in turn returns the active item
    IItem DropItem(IItem item);             //Called from within the character script
    IItem DropBatch(IItem item);            //Called when appropriate button is pressed. Drops the entire batch of a certain list of items
    IItem AddItemToSlot(IItem item);        //Called when user picks up an item in the environment
    void RearrangeSlots();                  //Called when user drags a certain slot onto a new slot
    void UseItem(IItem item, Character c);  //Called by the character that is holding the item. This function calls the use function within the item
    void ThrowItem(IItem item, Character c);//Called by the user. This function calls the throw function on the item itself
}

//Item has no information about the inventory
public interface IItem
{
    void PromptPickup();                    //Called by itself. Suggests player to pick up item by displaying a button
    void PickupItem(Character c);           //Called when button is pressed. This calls the player's pickup function
    void Use(Character c);                  //Called by the inventory. Every type of item has it's own special usage
    RaycastHit2D Throw(Vector3 targetPos);  //Called by the inventory. The item's use function is executed on impact

    void EnvironmentHit(GameObject g);      //Called by the throw function, if the item hits some platform or other interesting env. element
    void ItemHitCharacter(RaycastHit2D hit);//Called from within the throw function. This manages the type of hit encountered by item
    void Destroy();
}

public interface ITrap
{
    bool Activate(Character c);             //Called by itself when conditions are met or when failed to disarm
    void TryToDisarm(Character c);          //Called by the player when he tries to disarm a trap with appropriate item
    bool Disarm(Character c);               //Called by itself when disarm attempt is successful
}

public interface IVirtualJoystick
{
    void ActivateButton(Button button);     //Called from external script when button with certain functionality is needed
    void DeactivateButton(Button Button);   //Called from external script when the button is no longer needed
    void ChangeButtonSprite(Button button, Sprite sprite);
}

public interface IVirtualButton
{

}

[System.Serializable]
public class UserInput
{
    public float xInput = 0.0f;
    public bool jumpPressed = false;
    public bool jumpReleased = true;
    public bool pickUp = false;
    public bool throwItem = false;
    public bool useItem = false;
    public bool climbPressed = false;
    public bool absorbPressed = false;
    public bool doorOpenPressed = false;
    public bool specialButtonPressed = false;
    public bool specialButtonReleased = true;


    public bool JumpButton()
    {
        return false;
    }

    public bool JumpButtonDown()
    {
        return false;
    }

    public bool PickUpButton()
    {
        return false;
    }

    public bool PickUpButtonDown()
    {
        return false;
    }

    public bool ThrowItemButton()
    {
        return false;
    }
    public bool ThrowItemButtonDown()
    {
        return false;
    }
    public bool UseItemButton()
    {
        return false;
    }
    public bool UseItemButtonDown()
    {
        return false;
    }
}