export default function CompanyCard({ Children }) {
  return (
    <>
      {Children.map((company) => (
        <div key={company.id} className="bg-white p-4 rounded-lg shadow-lg">
          <img src={company.logo} alt="Go Bus" className="rounded-lg" />
          <h3 className="text-lg font-bold mt-2 text-center">{company.name}</h3>
        </div>
      ))}
    </>
  );
}
