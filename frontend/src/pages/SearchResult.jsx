import { useForm } from "react-hook-form";
import SearchForm from "../components/SearchForm";
import ResultsList from "../components/ResultCard";

const trips = [
  {
    company: "جو باص",
    price: 350,
    departure: "7:00 مساء",
    arrival: "7:00 مساء",
    location: "عبد المنعم رياض",
  },
  {
    company: "جو باص",
    price: 350,
    departure: "7:00 مساء",
    arrival: "7:00 مساء",
    location: "عبد المنعم رياض",
  },
  {
    company: "جو باص",
    price: 350,
    departure: "7:00 مساء",
    arrival: "7:00 مساء",
    location: "عبد المنعم رياض",
  },
  {
    company: "جو باص",
    price: 350,
    departure: "7:00 مساء",
    arrival: "7:00 مساء",
    location: "عبد المنعم رياض",
  },
  {
    company: "جو باص",
    price: 350,
    departure: "7:00 مساء",
    arrival: "7:00 مساء",
    location: "عبد المنعم رياض",
  },
  {
    company: "جو باص",
    price: 350,
    departure: "7:00 مساء",
    arrival: "7:00 مساء",
    location: "عبد المنعم رياض",
  },
];
export default function SearchResult() {
  return (
    <div className="p-6 flex flex-row w-full gap-2">
      <ResultsList className="grow" trips={trips} />
      <SearchForm />
    </div>
  );
}
