import logo from "../assets/file 1.png";

export default function Header() {
  return (
    <header className="bg-cyan-dark text-white  flex justify-around">
      <nav className="flex flex-row-reverse  items-center grow-3 justify-around text-lg font-semibold ">
        <a href="#" className="hover:underline">
          الرئيسية
        </a>
        <a href="#" className="hover:underline">
          الشركات
        </a>
        <a href="#" className="hover:underline">
          من نحن
        </a>
        <a href="#" className="hover:underline">
          تواصل معنا
        </a>
        <a className="bg-white font-normal text-black p-2 rounded" href="#">
          حسابى
        </a>
      </nav>
      <div className="grow-2"></div>
      <div className="flex items-center justify-center  grow-1">
        <h1 className="text-2xl p-2 font-bold">Tazkartk</h1>
        <img className="w-20" src={logo} />
      </div>
    </header>
  );
}
