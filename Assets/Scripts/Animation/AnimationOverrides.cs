using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR //UnityEditor只在editor 下使用 
using static UnityEditor.Progress;
#endif

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray = null;//这个是在外面配置好的数组，里面各项表明的某一个动画片段还有其身上的一些枚举 属性 （哪个部位 ， 是否有颜色变化效果 ， 这个动画来自哪种功能)


    //SO_AnimationType 本质上只是一个 AnimationClip ， 只是其内部有不少枚举值属性来表示 该 clip 是哪个部位 是用来hoe的 还是watering的 
    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionary__By__Animation;
    //这里的键 string 是 SO_AnimationType 内部几个枚举值的名字构成的，
    private Dictionary<string, SO_AnimationType> animationTypeDictionary__By__CompositeAttributeKey;

    private void Start()
    {
        animationTypeDictionary__By__Animation = new Dictionary<AnimationClip, SO_AnimationType>();


        //根据外面配置好的soAnimationTypeArray ， 初始化 上面定义的两个 字典
        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            //通过AnimationClip 来找到该 SO_AnimationType
            animationTypeDictionary__By__Animation.Add(item.animationClip , item);
        }

        animationTypeDictionary__By__CompositeAttributeKey = new Dictionary<string, SO_AnimationType>();

        foreach(SO_AnimationType item in soAnimationTypeArray)
        {
            //通过复合的属性名字 来找到 SO_AnimationType
            string key = item.characterPart.ToString() + item.partVariantColor.ToString() + item.partVarianType.ToString() + item.animationName.ToString();
            animationTypeDictionary__By__CompositeAttributeKey.Add(key, item);
        }
    }


    //这里CharacterAttribute 和 SO_AnimationType不同 ，不包含animation clip 只有 各种动画属性 （哪个部位 ，什么类型， 功能？）
    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributesList)
    {

        foreach(CharacterAttribute characterAttribute in characterAttributesList) 
        {
            Animator currentAnimator = null;

            //列表 不是 animationTypeDictionary__By__Animation ，是一个双AnimationClip 的列表，后面ApplyOverrides 函数要用的
            //里面每一项装载 的是 <目前正在用的 clip ， 替换的clip >
            List<KeyValuePair<AnimationClip , AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>> ();  

            string animatorSO_AssetName = characterAttribute.characterPart.ToString(); // 得到当前需要应用的是  body 的动画 还是 hair 的动画

            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>(); // 从player 身上得到各个部位（对应上面的body ） 的animator 

            foreach (Animator animator in animatorsArray)  // 找到需要替换的部位的animator 
            {
                if(animator.name == animatorSO_AssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            //如果这个动画替换的手部的动画，那么这里 currentAnimator 此处就找到了手部的 animator 

            //获取当前的runtimeAnimatorController
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            //从当前的runtimeAnimatorController 上得到这个部位（如手部） 的全部动画片段，放入列表中
            List<AnimationClip> animationList = new List<AnimationClip>(aoc.animationClips);

            //遍历该列表
            foreach(AnimationClip animationClip in animationList)
            {
                SO_AnimationType so_AnimationType;
                //根据目前遍历到的动画片段 在 animationTypeDictionary__By__Animation 寻找对应的 SO_AnimationType （就是有属性的animation clips）,找到的同时也把找到的 内容放入so_AnimationType 这个局部变量中
                //根据animationClip 找到的 SO_AnimationType 是目前正在使用的 SO_AnimationType 
                bool foundAnimation = animationTypeDictionary__By__Animation.TryGetValue(animationClip , out so_AnimationType);
                
                if(foundAnimation)  // 找到了就进行下一步
                {
                    //根据当前的characterAttribute 拼接 animationTypeDictionary__By__CompositeAttributeKey 要用的键 
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString()
                         + characterAttribute.partVarianType.ToString()  + so_AnimationType.animationName.ToString();
                    
                    SO_AnimationType swapSO_AnimationType;
                    //根据string key 在第二个字典中找需要替换的那个 SO_AnimationType
                    //根据 character的各项Attribute拼接而成的string key 找到的 SO_AnimationType 是用来替换的SO_AnimationType 也就是目标SO_AnimationType
                    bool foundSwapAnimation = animationTypeDictionary__By__CompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    if(foundSwapAnimation)
                    {
                        //同样，如果找到了对应属性的swapSO_AnimationType ，就把它的animationClip 记录下来
                        AnimationClip swap_animationClip = swapSO_AnimationType.animationClip;
                        // 把目前的animationClip 和 用来替换的 swap_animationClip 一起加入到 animsKeyValuePairList 列表中
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swap_animationClip));
                    }
                }
            }

            //实际实现动画覆盖的地方
            aoc.ApplyOverrides(animsKeyValuePairList); //用找到的替换目标的 list （比如从nocarry 到 carry）覆盖原来的animatoroverridecontroller
            currentAnimator.runtimeAnimatorController = aoc; //，， 帮当前正在运行的AnimatorController 设置为 替换过的aoc

        }
    }
}
