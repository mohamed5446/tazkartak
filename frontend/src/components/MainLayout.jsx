import Header from "./Header";
import Footer from "./Footer";

export default function MainLayout({ Children }) {
  return (
    <div className="flex flex-col h-screen">
      <Header />

      <div className="flex-auto">{Children}</div>


     

      <Footer />
    </div>
  );
}
