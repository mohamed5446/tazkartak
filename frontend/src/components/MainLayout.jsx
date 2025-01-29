import Header from "./Header";
import Footer from "./Footer";

export default function MainLayout({ Children }) {
  return (
    <div>
      <Header />
     <main>{Children}</main> 
      <Footer />
    </div>
  );
}
