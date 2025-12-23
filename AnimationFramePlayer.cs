using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimationFramePlayer : MonoBehaviour
{
    public AnimationCombinedFrames frameData;

    new MeshRenderer renderer;

    Material material;

    int currentPlayIndex = -1;

    private void OnValidate()
    {
        if ( renderer == null ) renderer = GetComponent<MeshRenderer>();

        if( renderer != null ) // && frameData != null && currentPlayIndex == -1 )
        {
            material = renderer.sharedMaterial;

            Play( 0 );
        }
    }

    public void Play( int index )
    {
        if( material == null ) return;

        if( currentPlayIndex == index ) return;

        currentPlayIndex = index;

        if( currentPlayIndex < 0 )
        {
            // stop animation 

            UpdateMaterial();
        }
        else
        {
            var item = frameData.data[ currentPlayIndex ];

            UpdateMaterial( item.frames, item.offset, item.duration );
        }
    }

    void UpdateMaterial( int frames = 0, int offset = 0, float duration = 0 )
    {
        material.DisableKeyword("_TIMERMODE_SCRIPT");
        material.EnableKeyword("_TIMERMODE_SHADER");
        material.SetFloat( "_Timer" , 0 );

        material.SetFloat( "_Frames" , frames );
        material.SetFloat( "_Offset" , offset );
        material.SetFloat( "_Duration" , duration );
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(AnimationFramePlayer))]
    class Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = ( AnimationFramePlayer ) target;

            if( script.frameData == null ) return;

            GUILayout.Space(10);

            if( script.currentPlayIndex == -1 )
            {
                GUILayout.Label("Paused");
            }
            else
            {
                GUILayout.Label("Playing index: " + script.currentPlayIndex );
            }

            GUILayout.Space(10);

            GUILayout.Label("Select animation to play: ");

            int index = -1 ;

            foreach( var item in script.frameData.data )
            {
                index ++ ;

                bool b = index == script.currentPlayIndex;

                using( new EditorGUI.DisabledGroupScope( b ) )
                {
                    if( GUILayout.Button( item.name ) )
                    {
                        script.Play( index );
                    }
                }

            }
        }
    }

#endif
}
