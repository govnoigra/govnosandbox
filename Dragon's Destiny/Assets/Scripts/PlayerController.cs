using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] //нужен для того, чтобы переменные из класса Boundary отображались в инспекторе юнити
public class Boundary //класс, во котором находятся переменные, определяющие границы поля
{
    public float xMin, xMax, zMin, zMax; //переменные, которые являются ограничением игрового поля, чтобы не вылететь за ее пределы. y нам не нужен, потому что значения y в нашей игры не меняется при передвижениях корабля

}




public class PlayerController : MonoBehaviour
{
    public float Speed = 10; //переменная скорости. public означает, что переменная публичная и ее параметр можно менять в самом юнити
    public Boundary boundary; // переменная boundary принадлежит типу Boundary. Хоть и публичная, но не отображается в юинит, смотри перед классом боундари, там систем сериализэйбл
    public float tilt; //хранит в себе коэффициент угла наклона корабля при движении

    public Quaternion calibrationQuaternion;

    public void Start()
    {
        CalibrateAccelerometr();
    }

    public void CalibrateAccelerometr()
    {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
    }

    public Vector3 FixAcceleration (Vector3 acceration)
    {
        Vector3 fixedAcceleration = calibrationQuaternion * acceration;
        return fixedAcceleration;

    }

    private void FixedUpdate() //функция расчитывает физику, потом отрысовывает картинку( в просто Update наоборот 
    {
        Vector3 accelerationRaw = Input.acceleration;
        Vector3 acceleration = FixAcceleration(accelerationRaw);

        GetComponent<Rigidbody>().rotation = Quaternion.Euler( //необходим для создания наклона корабля в сторону при движении
            0f, //значение x = 0 , так как мы не будем вращать вдоль этих осей
            0f, //значения y = 0, так как мы не будем вращать вдоль этих осей
            GetComponent<Rigidbody>().velocity.x * -tilt //свяжем переменную z со скоростью наклона и скоростью перемещения. velocity.x потому что нужна скорость перемещения вправо и влево. -tilt потому что с помощью - будет поворот наклона в обе стороны
            );

        GetComponent<Rigidbody>().velocity = new Vector3(acceleration.x, 0f, acceleration.y) * Speed;//двигает корабль в зависимоти от полученных значений GetAxis. Умножение на Speed позволяет получить скорость больше чем 1 или -1

        GetComponent<Rigidbody>().position = new Vector3
            (
                Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), //структуа Mathf.Clamp содержит много матем.формул. Нужна, чтобы ограничить переменную х от xMin до xMax. 0f означает, что y равен 0, ибо он нам не нужен. Переменные являются часть класса Boundary
                0.0f,
                Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax) //структуа Mathf.Clamp содержит много матем.формул. Нужна, чтобы ограничить переменную z от zMin до zMax. 0f означает, что y равен 0, ибо он нам не нужен. Переменные являются часть класса Boundary
            );

    }
}
