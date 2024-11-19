using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class SignUpPanel : MonoBehaviour
{
    // 이용자가 입력하는 값을 받아 저장하는 InputField
    [SerializeField] TMP_InputField emailInputfield;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TMP_InputField passwordConfirmInputField;

    public void SignUp()
    {
        // InputField에 입력된 각 값을 이름에 맞는 string 자료형에 각자 저장하여 사용한다.
        string email = emailInputfield.text;
        string pass = passwordInputField.text;
        string confirm = passwordConfirmInputField.text;

        // 이메일 란에 입력값이 없었을 경우,
        if (email.IsNullOrEmpty()) // or == "";
        {
            // 입력을 재시도 하게 한다.
            Debug.LogWarning("이메일 입력 후 다시 실행해주세요.");
            return;
        }
        // 패스워드 란의 입력값과, 패스워드 확인 란의 입력 값이 동일하지 않을 경우,
        if (pass != confirm)
        {
            // 입력을 재시도 하게 한다.
            Debug.LogWarning("패스워드가 일치하지 않습니다.");
            return;
        }

        // 위 InputField에 입력된 정보값(이메일, 패스워드)을 가진 유저를 서버 데이터에 등록할 수 있도록 요청을 보낸 후,
        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(email, pass)
        // 해당 요청이 완료되면 아래 작성된 소스코드를 실행한다.
            .ContinueWithOnMainThread(task =>
        {
            //  ( ).IsCanceled = 요청 취소
            if (task.IsCanceled)
            {
                // 요청이 취소되었다면, 넘어간다.
                Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            // ( ).IsFaulted = 요청 실패
            if (task.IsFaulted)
            {
                // 요청이 실패하였다면, 실패 사유를 콘솔창에 띄운 후, 넘어간다.
                Debug.Log($"CreateUserWithEmailAndPasswordAsync encountered an error :" + task.Exception);
                return;
            }


            // 위 두 경우를 넘기고 요청에 성공하였을 경우,

            // 해당 인증 결과를 저장하고, 
            AuthResult result = task.Result;
            // 콘솔창에 입력된 이메일과 비밀번호를 지닌 유저가 정상적으로 생성되었다는 메세지를 띄운다.
            Debug.Log($"Firebase user created successfully : {result.User.DisplayName} ({result.User.UserId})");
            // 회원가입이 완료되었으므로, 회원가입 화면을 비활성화 하여 로그인 화면으로 되돌아간다.
            gameObject.SetActive(false);

        });
    }
}
