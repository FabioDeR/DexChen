﻿namespace DexChen.Domain.Dtos;

public class UserMetaDtos
{
    public string email { get; set; }
    public bool email_verified { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public bool phone_verified { get; set; }
    public string sub { get; set; }
}
