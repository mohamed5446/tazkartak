import logo from "../assets/images/tazkartak.jpg";

export default function About() {
  return (
    <div className="m-4">
      <div className="bg-cyan-dark  text-white p-4 rounded-3xl">
        <div className="text-end p-4 ">
          <h1 className="text-4xl font-bold ">Tazkartak</h1>
          <p className="mt-3 ">
            شركة مصرية تجمع كل شركات الاتوبيس داخل الدولة لتسهيل عملية حجز
            التذاكر
          </p>
        </div>
        <h2 className="text-2xl font-bold text-end">اهم الأهداف</h2>

        <div className="flex  justify-between px-4 py-4">
          <div className="flex  ">
            <img className="w-96 rounded-lg mt-4 ml-4" src={logo} alt="Logo" />
          </div>

          <div className="flex gap-4 items-start text-right direction-rtl">
            <ul className="space-y-2 ">
              <li>الرئيسية</li>
              <li>الشركات</li>
              <li>من نحن</li>
              <li>تواصل معنا</li>
              <li>حسابي</li>
            </ul>

            <ul className="space-y-2">
              <li>-1</li>
              <li>-2</li>
              <li>-3</li>
              <li>-4</li>
              <li>-5</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
}
