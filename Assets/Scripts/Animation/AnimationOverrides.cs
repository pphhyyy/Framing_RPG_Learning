using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR //UnityEditorֻ��editor ��ʹ�� 
using static UnityEditor.Progress;
#endif

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;//��������������úõ����飬������������ĳһ������Ƭ�λ��������ϵ�һЩö�� ���� ���ĸ���λ �� �Ƿ�����ɫ�仯Ч�� �� ��������������ֹ���)


    //SO_AnimationType ������ֻ��һ�� AnimationClip �� ֻ�����ڲ��в���ö��ֵ��������ʾ �� clip ���ĸ���λ ������hoe�� ����watering�� 
    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionary__By__Animation;
    //����ļ� string �� SO_AnimationType �ڲ�����ö��ֵ�����ֹ��ɵģ�
    private Dictionary<string, SO_AnimationType> animationTypeDictionary__By__CompositeAttributeKey;

    private void Start()
    {
        animationTypeDictionary__By__Animation = new Dictionary<AnimationClip, SO_AnimationType>();


        //�����������úõ�soAnimationTypeArray �� ��ʼ�� ���涨������� �ֵ�
        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            //ͨ��AnimationClip ���ҵ��� SO_AnimationType
            animationTypeDictionary__By__Animation.Add(item.animationClip , item);
        }

        animationTypeDictionary__By__CompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach(SO_AnimationType item in soAnimationTypeArray)
        {
            //ͨ�����ϵ��������� ���ҵ� SO_AnimationType
            string key = item.characterPart.ToString() + item.partVariantColor.ToString() + item.partVarianType.ToString() + item.animationName.ToString();
            animationTypeDictionary__By__CompositeAttributeKey.Add(key, item);
        }
    }


    //����CharacterAttribute �� SO_AnimationType��ͬ ��������animation clip ֻ�� ���ֶ������� ���ĸ���λ ��ʲô���ͣ� ���ܣ���
    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributesList)
    {

        foreach(CharacterAttribute characterAttribute in characterAttributesList) 
        {
            Animator currentAnimator = null;

            //�б� ���� animationTypeDictionary__By__Animation ����һ��˫AnimationClip ���б�����ApplyOverrides ����Ҫ�õ�
            //����ÿһ��װ�� ���� <Ŀǰ�����õ� clip �� �滻��clip >
            List<KeyValuePair<AnimationClip , AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>> ();  

            string animatorSO_AssetName = characterAttribute.characterPart.ToString(); // �õ���ǰ��ҪӦ�õ���  body �Ķ��� ���� hair �Ķ���

            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>(); // ��player ���ϵõ�������λ����Ӧ�����body �� ��animator 

            foreach (Animator animator in animatorsArray)  // �ҵ���Ҫ�滻�Ĳ�λ��animator 
            {
                if(animator.name == animatorSO_AssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            //�����������滻���ֲ��Ķ�������ô���� currentAnimator �˴����ҵ����ֲ��� animator 

            //��ȡ��ǰ��runtimeAnimatorController
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            //�ӵ�ǰ��runtimeAnimatorController �ϵõ������λ�����ֲ��� ��ȫ������Ƭ�Σ������б���
            List<AnimationClip> animationList = new List<AnimationClip>(aoc.animationClips);

            //�������б�
            foreach(AnimationClip animationClip in animationList)
            {
                SO_AnimationType so_AnimationType;
                //����Ŀǰ�������Ķ���Ƭ�� �� animationTypeDictionary__By__Animation Ѱ�Ҷ�Ӧ�� SO_AnimationType �����������Ե�animation clips��,�ҵ���ͬʱҲ���ҵ��� ���ݷ���so_AnimationType ����ֲ�������
                //����animationClip �ҵ��� SO_AnimationType ��Ŀǰ����ʹ�õ� SO_AnimationType 
                bool foundAnimation = animationTypeDictionary__By__Animation.TryGetValue(animationClip , out so_AnimationType);
                
                if(foundAnimation)  // �ҵ��˾ͽ�����һ��
                {
                    //���ݵ�ǰ��characterAttribute ƴ�� animationTypeDictionary__By__CompositeAttributeKey Ҫ�õļ� 
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString()
                         + characterAttribute.partVarianType.ToString()  + so_AnimationType.animationName.ToString();
                    
                    SO_AnimationType swapSO_AnimationType;
                    //����string key �ڵڶ����ֵ�������Ҫ�滻���Ǹ� SO_AnimationType
                    //���� character�ĸ���Attributeƴ�Ӷ��ɵ�string key �ҵ��� SO_AnimationType �������滻��SO_AnimationType Ҳ����Ŀ��SO_AnimationType
                    bool foundSwapAnimation = animationTypeDictionary__By__CompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    if(foundSwapAnimation)
                    {
                        //ͬ��������ҵ��˶�Ӧ���Ե�swapSO_AnimationType ���Ͱ�����animationClip ��¼����
                        AnimationClip swap_animationClip = swapSO_AnimationType.animationClip;
                        // ��Ŀǰ��animationClip �� �����滻�� swap_animationClip һ����뵽 animsKeyValuePairList �б���
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swap_animationClip));
                    }
                }
            }

            //ʵ��ʵ�ֶ������ǵĵط�
            aoc.ApplyOverrides(animsKeyValuePairList); //���ҵ����滻Ŀ��� list �������nocarry �� carry������ԭ����animatoroverridecontroller
            currentAnimator.runtimeAnimatorController = aoc; //���� �ﵱǰ�������е�AnimatorController ����Ϊ �滻����aoc

        }
    }
}
