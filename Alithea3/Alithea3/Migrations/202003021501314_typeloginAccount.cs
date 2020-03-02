namespace Alithea3.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class typeloginAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserAccounts", "loginType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserAccounts", "loginType");
        }
    }
}
