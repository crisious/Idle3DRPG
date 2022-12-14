using UnityEngine;

namespace MultiplayerARPG
{
    public class ActivateButtonActivator : MonoBehaviour
    {
        public GameObject[] activateObjects;

        private bool canActivate;
        private PlayerCharacterController controller;
        private ShooterPlayerCharacterController shooterController;

        private void LateUpdate()
        {
            canActivate = false;

            controller = BasePlayerCharacterController.Singleton as PlayerCharacterController;
            shooterController = BasePlayerCharacterController.Singleton as ShooterPlayerCharacterController;

            if (controller != null)
            {
                canActivate = controller.ActivatableEntityDetector.players.Count > 0 ||
                    controller.ActivatableEntityDetector.npcs.Count > 0 ||
                    controller.ActivatableEntityDetector.buildings.Count > 0;
            }


            if (shooterController != null && shooterController.SelectedEntity != null)
            {
                if ((shooterController.SelectedEntity is BasePlayerCharacterEntity || shooterController.SelectedEntity is NpcEntity) &&
                    Vector3.Distance(shooterController.SelectedEntity.CacheTransform.position, shooterController.PlayerCharacterEntity.CacheTransform.position) <= GameInstance.Singleton.conversationDistance)
                {
                    canActivate = true;
                }

                if (!canActivate)
                {
                    BuildingEntity buildingEntity = shooterController.SelectedEntity as BuildingEntity;
                    if (buildingEntity != null && !buildingEntity.IsBuildMode && buildingEntity.Activatable)
                    {
                        canActivate = true;
                    }
                }
            }

            foreach (GameObject obj in activateObjects)
            {
                obj.SetActive(canActivate);
            }
        }
    }
}
