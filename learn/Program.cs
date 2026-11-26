// 클래스를 민들어야 합니다.


//블록즈

// 추상 클래스 공통적인 기ㅣ능들을 뺴낸다
// 테트리스블록이 공통적으로 갖는 특징은?
// 회전, 낙하, 한 줄 삭제, 겹침 금지
// 색상, 모양

// I is 블록 -> 상속
// O -> 회전 할 수 도 있다 -> 회전 가능한 IRollable 인터페이스

블록 I = new 블록("I");
블록 T = new 블록("T");
블록 L = new 블록("L"); 
블록 Z = new 블록("Z"); 
블록 O = new 블록("O");


class 블록
{
    public string shape = "ㅁ";

    public 블록(string shape)
    {
        this.shape = shape;
    }
}
 // 블록의 모든 성질을 가지면서 새로운 기능을 한번 추가
 // 시간제한이 있어 일전 시간 후 파괴 위에서 블록이 자동 하강
 
class TimeBlock :
{
    
}