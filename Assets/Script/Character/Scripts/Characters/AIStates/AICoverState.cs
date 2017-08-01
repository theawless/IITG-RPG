using System;
using UnityEngine;
using AIManager_namespace;

public class AICoverState<T> : IAIStateManager where T: AIManager
{
    private T m_aiGunnerOrCombat;
    private float m_sqrDistanceFromCoverPosition = float.MaxValue;
    private Vector3 m_trackCharacterPosition;

    public AICoverState(T aiManager)
    {
        m_aiGunnerOrCombat = aiManager;
    }

    public void UpdateCurrentState()
    {
        Debug.Log("Cover State..." + m_aiGunnerOrCombat.m_CharType.ToString());
        m_aiGunnerOrCombat.m_animator.SetFloat("Strafe", 1f);

        UpdateCoverState();
        ChangeStateConditions();
    }

    private void UpdateCoverState()
    {
        m_aiGunnerOrCombat.CheckEveryCharacter();
        m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter = m_aiGunnerOrCombat.FindNearestOpponentGameObjectWithType();

        if (m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject != null && m_aiGunnerOrCombat.m_isInCover)
        {
            m_trackCharacterPosition = m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject.transform.position;
        }

        m_sqrDistanceFromCoverPosition = Vector3.SqrMagnitude(m_aiGunnerOrCombat.transform.position - m_aiGunnerOrCombat.m_coverPositionScript.transform.position);

        if (m_sqrDistanceFromCoverPosition <= 2f)
        {
            m_aiGunnerOrCombat.m_isInHighCover = m_aiGunnerOrCombat.CheckForLowOrHighCover();
        }

        if (m_aiGunnerOrCombat.m_isInHighCover)
        {
            //m_aiGunnerOrCombat.m_animator.SetBool()
        }

        if (m_sqrDistanceFromCoverPosition <= 2f * m_aiGunnerOrCombat.m_ThresholdDistance)
        {
            m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.IdleAnimation;
            m_aiGunnerOrCombat.m_isInCover = true;
            Vector3 fromCoverToOpponentDirection = m_trackCharacterPosition - m_aiGunnerOrCombat.m_coverPositionScript.transform.position;
            Debug.DrawRay(m_aiGunnerOrCombat.m_coverPositionScript.transform.position + Vector3.up * 0.5f, fromCoverToOpponentDirection.normalized * 3f, Color.magenta, 3f);

            if (!Physics.Raycast(m_aiGunnerOrCombat.m_coverPositionScript.transform.position + Vector3.up * 0.5f, fromCoverToOpponentDirection, 3f, m_aiGunnerOrCombat.m_EnvironmentLayerForCover, QueryTriggerInteraction.Ignore))
            {
                FindCoverPosition();
            }
            else
            {
                Debug.Log("Does hit the cover layerMask");
                m_aiGunnerOrCombat.m_currentDestinationPoint = m_aiGunnerOrCombat.m_mainDestinationPoint;
            }
        }
        else if (m_aiGunnerOrCombat.m_navMeshPath.corners.Length <= 2)
        {
            m_aiGunnerOrCombat.m_isInCover = false;
            m_aiGunnerOrCombat.m_currentDestinationPoint = m_aiGunnerOrCombat.m_mainDestinationPoint;
            m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.RunningAnimation;
        }
        else
        {
            m_aiGunnerOrCombat.m_currentAnimationState = AnimationState.RunningAnimation;
        }
        UpdateAnimation();
        m_aiGunnerOrCombat.FollowAlongNavMeshPath(m_aiGunnerOrCombat.m_navMeshPath, m_trackCharacterPosition, true);
    }

    private void UpdateAnimation()
    {
        m_aiGunnerOrCombat.m_animator.SetBool("IsCrouch", m_aiGunnerOrCombat.m_isInHighCover);
        m_aiGunnerOrCombat.m_animator.SetBool("IsCover", m_aiGunnerOrCombat.m_isInCover);
    }

    private void ChangeStateConditions()
    {
        if (!m_aiGunnerOrCombat.m_isInCover)
        {
            if (m_aiGunnerOrCombat.m_nearestOpponentVisibleCharacter.characterGameObject != null)
            {
                if (m_aiGunnerOrCombat.m_CharType == CharacterType.EnemyGunner || m_aiGunnerOrCombat.m_CharType == CharacterType.PlayerGunnerCampanion)
                {
                    if (m_sqrDistanceFromCoverPosition <= 2f * m_aiGunnerOrCombat.m_ThresholdDistance)
                    {
                        ChangeToGunFireState();
                    }
                }
                else
                {
                    ChangeToChaseState();
                }
                return;
            }
            else
            {
                ChangeToSearchState();
            }
        }
    }

    private void FindCoverPosition()
    {
        if (m_aiGunnerOrCombat.CanFindBestCoverPosition(ref m_aiGunnerOrCombat.m_coverPositionScript))
        {
            m_aiGunnerOrCombat.m_coverPositionScript.m_isOccupied = true;
            m_aiGunnerOrCombat.m_mainDestinationPoint = m_aiGunnerOrCombat.m_coverPositionScript.transform.position;
            m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
            Debug.Log("I can find the cover Position");
        }
        else
        {
            Debug.Log("Can't find the cover Position");
            m_aiGunnerOrCombat.m_currentDestinationPoint = m_aiGunnerOrCombat.m_mainDestinationPoint;
        }
    }

    public void ChangeToCoverState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToUnwareState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToInvestigateState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToPatrolState()
    {
        throw new NotImplementedException();
    }

    public void ChangeToChaseState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_chaseAIState;
        ResetCoverState();
    }

    public void ChangeToBoxingState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_boxingAIState;
        m_aiGunnerOrCombat.StartCoroutine(m_aiGunnerOrCombat.ChangeAnimationLayer(1, 0));
    }

    public void ChangeToGunFireState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_gunFireAIState;
    }

    public void ChangeToSearchState()
    {
        m_aiGunnerOrCombat.m_currentAIState = m_aiGunnerOrCombat.m_searchAIState;
        m_aiGunnerOrCombat.m_mainDestinationPoint = m_trackCharacterPosition;
        m_aiGunnerOrCombat.m_navMeshPath = m_aiGunnerOrCombat.CalculateNavmeshPath(m_aiGunnerOrCombat.m_mainDestinationPoint);
        ResetCoverState();
    }

    private void ResetCoverState()
    {
        m_aiGunnerOrCombat.m_animator.SetFloat("Strafe", 0f);
    }
}
