using UnityEngine;

public interface IPickup
{
	void Use(Vector3 attackDirection);

	bool IsPickedUp();
}
