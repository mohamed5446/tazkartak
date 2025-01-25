import Header from "./Header";
import Footer from "./Footer";

export default function MainLayout({ Children }) {
  return (
    <div>
      <Header />
      {Children}
      <Footer />
    </div>
  );
}
