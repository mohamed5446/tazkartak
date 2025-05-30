
public class paymobresponse
{
    public Obj obj { get; set; }

}
public class Obj
{
    public int id { get; set; }
    public bool pending { get; set; }
    public int amount_cents { get; set; }
    public bool success { get; set; }
    public bool is_auth { get; set; }
    public bool is_capture { get; set; }
    public bool is_standalone_payment { get; set; }
    public bool is_voided { get; set; }
    public bool is_refunded { get; set; }
    public bool is_3d_secure { get; set; }
    public int integration_id { get; set; }
    public bool has_parent_transaction { get; set; }
    public Order order { get; set; }
    public DateTime created_at { get; set; }
    public string currency { get; set; }
    public Source_Data source_data { get; set; }

    public Payment_Key_Claims payment_key_claims { get; set; }
    public bool error_occured { get; set; }

    public int owner { get; set; }
}

public class Order
{
    public int id { get; set; }

}


public class Source_Data
{
    public string pan { get; set; }
    public string type { get; set; }
    public object tenure { get; set; }
    public string sub_type { get; set; }
}


public class Payment_Key_Claims
{
    public Extra extra { get; set; }

}

public class Extra
{
    public List<int> seats { get; set; }
    public int tripid { get; set; }
    public int userid { get; set; }
}

