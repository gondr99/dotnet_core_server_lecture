namespace GameServer.Dtos;


/// <summary>
/// 채팅 메시지 페이로드 객체
/// record는 불변 객체로 불변 데이터를 표기하는데 사용한다. (한줄로 선언하는 형태를 가진다.)
/// record는 string처럼 불변한 객체로서 힙에 할당되기 때문에, 중간에 값을 바꾸는게 안된다.
/// 이녀석은 같다 라는 동등성 판단이 값을 기반으로 이뤄진다.
/// 레코드는 튜플처럼 분해 가능하다.
/// </summary>
/// <param name="Channel">채팅 채널이고 general 또는 guild[id] 가 들어간다.</param>
/// <param name="Nickname">발신자의 닉네임(서버가 JWT 클레임을 통해 자동으로 채운다.)</param>
/// <param name="Text">메시지 본문</param>
public record ChatMessage(string Channel, string Nickname, string Text);
