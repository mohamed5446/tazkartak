import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faWhatsapp } from "@fortawesome/free-brands-svg-icons";

export default function ContactUs() {
  const phoneNumber = "201019350517"; 

  return (
    <div className="flex flex-col items-center justify-center relative  bg- rounded-3xl " style={{width:"30%" ,height:"40%",position:"absolute",top:"48%",left:"50%",transform:"translate(-50%,-50%)",backgroundColor:"rgb(60, 107, 125)" }}>
     

     
      <div className="bottom-3 right-6 ">
        <a
          href={`https://wa.me/${phoneNumber}`}
          target="_blank"
          rel="noopener noreferrer"
          className=" hover:text-green-500 text-white rounded-2xl shadow-xl flex flex-col items-center justify-center w-28 h-28 transition-all duration-300"
        >
         
          <FontAwesomeIcon icon={faWhatsapp} className="text-7xl mb-3 " />
       
        
        </a>
      </div>
      <h1 className="text-4xl font-bold mb-3 text-white ">تواصل معنا</h1>
    </div>
  );
};
