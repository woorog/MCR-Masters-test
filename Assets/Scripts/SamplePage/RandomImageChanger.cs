using UnityEngine;
using UnityEngine.UI;

public class RandomImageChanger : MonoBehaviour
{
    // 변경할 UI 이미지 컴포넌트를 할당합니다.
    public Image targetImage;

    // 무작위로 선택될 스프라이트 배열입니다.
    public Sprite[] sprites;

    // 버튼 클릭 시 호출되는 함수입니다.
    public void ChangeImage()
    {
        // 스프라이트 배열이나 targetImage가 비어있는 경우 함수 종료
        if (sprites == null || sprites.Length == 0 || targetImage == null)
        {
            Debug.LogWarning("스프라이트 배열이나 targetImage가 설정되지 않았습니다.");
            return;
        }

        // 배열의 길이만큼 무작위 인덱스 선택
        int randomIndex = Random.Range(0, sprites.Length);
        // 선택한 스프라이트로 이미지 변경
        targetImage.sprite = sprites[randomIndex];
    }
}
