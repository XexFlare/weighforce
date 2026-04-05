import { createWebHistory, createRouter } from "vue-router";
import Transit from "../views/Transit.vue";
import Link from "../views/Link.vue";
import Speed from "../views/Speed.vue";
import TruckDatabase from "../views/TruckDatabase.vue";
import Login from "../views/Login.vue";
import Logout from "../views/Logout.vue";
import TransporterDet from "../views/TransporterDet.vue";
import TransportDet from "../views/TransportDet.vue";
import Transporters from "../views/Transporters.vue";
import Products from "../views/Products.vue";
import ProductDet from "../views/ProductDet.vue";
import Locations from "../views/Locations.vue";
import LocationDet from "../views/LocationDet.vue";
import Countries from "../views/Countries.vue";
import CountryDet from "../views/CountryDet.vue";
import Units from "../views/Units.vue";
import UnitDet from "../views/UnitDet.vue";
import TransportInstructions from "../views/TransportInstructions.vue";
import Menu from "../views/Menu.vue";
import User from "../views/User.vue";
import Upload from "../views/Upload.vue";
import SendMail from "../views/SendMail.vue";
import Reports from "../views/Reports.vue";
import Overweight from "../views/Overweight.vue";
import Underweight from "../views/Underweight.vue";
import Contracts from "../views/Contracts.vue";
import Contract from "../views/Contract.vue";
import Booking from "../views/Booking.vue";
import Receival from "../views/Receival.vue";
import Receivals from "../views/Receivals.vue";
import MailingList from "../views/MailingList.vue";
import Settings from "../views/Settings.vue";
import Users from "../views/Users.vue";
import OsrReport from "../views/OsrReport.vue";
import DailySummary from "../views/DailySummary.vue";
import WeeklyDispatchedTrucks from "../views/WeeklyDispatchedTrucks.vue";
import AnnualDispatchReport from "../views/AnnualDispatchReport.vue";
import DailyReceivals from "../views/DailyReceivals.vue";
import WeeklyReceivals from "../views/WeeklyReceivals.vue";
import AnnualReceivals from "../views/AnnualReceivals.vue";
import ShortageReport from "../views/ShortageReport.vue";
import Tolerance from "../views/Tolerance.vue";
import Sync from "../views/Sync.vue";
import { ApplicationPaths, LoginActions, LogoutActions } from '../auth/ApiAuthorizationConstants.js';
import AuthRoute from '../auth/AuthRoute.vue';

const routes = [
    {
        path: "/",
        name: "Menu",
        component: AuthRoute,
        props: { component: Menu },
    },
    {
        path: "/settings",
        name: "Settings",
        component: AuthRoute,
        props: { component: Settings },
        children: [
            {
                path: "sync",
                name: "Sync",
                component: Sync,
            },
            {
                path: "user",
                name: "User",
                component: User,
            },
            {
                path: "users",
                name: "Users",
                component: Users,
                props: { roles: ['admin'] },
            },
            {
                path: "mailing-list",
                name: "Mailing List",
                component: MailingList,
                props: { roles: ['admin'] },
            },
            {
                path: "transport-instructions",
                name: "Transport Instructions",
                component: TransportInstructions,
                props: { roles: ['admin'] },
            },
            {
                path: "transport-instructions/:id",
                name: "Transport Instruction Details",
                component: TransportDet,
                props: { roles: ['admin'] },
            },
            {
                path: "transport-instructions/new",
                name: "New Transport Instruction",
                component: TransportDet,
                props: { roles: ['admin'] },
            },
            {
                path: "transporters",
                name: "Transporters",
                component: Transporters,
                props: { roles: ['admin'] },
            },
            {
                path: "transporters/:id",
                name: "Transporter Details",
                component: TransporterDet,
                props: { roles: ['admin'] },
            },
            {
                path: "transporters/new",
                name: "New Transporter",
                component: TransporterDet,
                props: { roles: ['admin'] },
            },
            {
                path: "products",
                name: "Products",
                component: Products,
                props: { roles: ['admin'] },
            },
            {
                path: "products/:id",
                name: "Product Details",
                component: ProductDet,
                props: { roles: ['admin'] },
            },
            {
                path: "products/new",
                name: "New Product",
                component: ProductDet,
                props: { roles: ['admin'] },
            },
            {
                path: "locations",
                name: "Locations",
                component: Locations,
                props: { roles: ['admin'] },
            },
            {
                path: "locations/:id",
                name: "Location Details",
                component: LocationDet,
                props: { roles: ['admin'] },
            },
            {
                path: "locations/new",
                name: "New Location",
                component: LocationDet,
                props: { roles: ['admin'] },
            },
            {
                path: "countries",
                name: "Countries",
                component: Countries,
                props: { roles: ['admin'] },
            },
            {
                path: "countries/:id",
                name: "Country Details",
                component: CountryDet,
                props: { roles: ['admin'] },
            },
            {
                path: "countries/new",
                name: "New Country",
                component: CountryDet,
                props: { roles: ['admin'] },
            },
            {
                path: "units",
                name: "Units",
                component: Units,
                props: { roles: ['admin'] },
            },
            {
                path: "units/:id",
                name: "Unit Details",
                component: UnitDet,
                props: { roles: ['admin'] },
            },
            {
                path: "units/new",
                name: "New Unit",
                component: UnitDet,
                props: { roles: ['admin'] },
            },
            {
                path: "contracts",
                name: "Contracts",
                component: Contracts,
                props: { roles: ['manager'] },
            },
            {
                path: "contracts/:id",
                name: "Contract",
                component: Contract,
                props: { roles: ['manager'] },
            },
            {
                path: "contracts/new",
                name: "New Contract",
                component: Contract,
                props: { roles: ['manager'] },
            },
            {
                path: "upload",
                name: "Upload",
                component: Upload,
                props: { roles: ['upload'] },
            },
            {
                path: "send",
                name: "Send Report",
                component: SendMail,
                props: { roles: ['upload'] },
            },
            {
                path: "",
                name: "profile",
                component: User,
            },
        ]
    },
    {
        path: "/receival",
        name: "New Manual Receival",
        component: AuthRoute,
        props: { component: Receival, roles: ['dispatch', 'manager'] },
    },
    {
        path: "/receivals",
        name: "Manual Receivals",
        component: AuthRoute,
        props: { component: Receivals, roles: ['dispatch', 'manager'] },
    },
    {
        path: "/booking",
        name: "New Booking",
        component: AuthRoute,
        props: { component: Booking, roles: ['dispatch', 'manager'] },
    },
    {
        path: "/transit",
        name: "Transit",
        component: AuthRoute,
        props: { component: Transit, roles: ['operator'] },
    },
    {
        path: "/link",
        name: "Link Rail",
        component: AuthRoute,
        props: { component: Link, roles: ['link'] },
    },
    {
        path: "/speed",
        name: "Wagons Speed Alert",
        component: AuthRoute,
        props: { component: Speed, roles: ['manager', 'link'] },
    },
    {
        path: "/reports/osr",
        name: "Liwonde Rail to OSR Report",
        component: AuthRoute,
        props: { component: OsrReport, roles: ['manager'] },
    },
    {
        path: "/reports/daily-dispatches",
        name: "Daily Dispatches",
        component: AuthRoute,
        props: { component: DailySummary, roles: ['manager'] },
    },
    {
        path: "/reports/weekly-dispatches",
        name: "Weekly Dispatches",
        component: AuthRoute,
        props: { component: WeeklyDispatchedTrucks, roles: ['manager'] },
    },
    {
        path: "/reports/annual-dispatches",
        name: "Annual Dispatch Summary",
        component: AuthRoute,
        props: { component: AnnualDispatchReport, roles: ['manager'] },
    },
    {
        path: "/reports/daily-receivals",
        name: "Daily Receivals",
        component: AuthRoute,
        props: { component: DailyReceivals, roles: ['manager'] },
    },
    {
        path: "/reports/weekly-receivals",
        name: "Weekly Receivals",
        component: AuthRoute,
        props: { component: WeeklyReceivals, roles: ['manager'] },
    },
    {
        path: "/reports/annual-receivals",
        name: "Annual Receival Summary",
        component: AuthRoute,
        props: { component: AnnualReceivals, roles: ['manager'] },
    },
    {
        path: "/reports/shortages",
        name: "Shortage Report",
        component: AuthRoute,
        props: { component: ShortageReport, roles: ['manager'] },
    },
    {
        path: "/reports/limits",
        name: "Tolerance",
        component: AuthRoute,
        props: { component: Tolerance, roles: ['manager'] },
    },
    {
        path: "/reports/over",
        name: "Overweight",
        component: AuthRoute,
        props: { component: Overweight, roles: ['manager'] },
    },
    {
        path: "/reports/under",
        name: "Underweight",
        component: AuthRoute,
        props: { component: Underweight, roles: ['manager'] },
    },
    {
        path: "/reports",
        name: "Reports",
        component: AuthRoute,
        props: { component: Reports, roles: ['manager'] },
    },
    {
        path: "/trucks",
        name: "Trucks",
        component: AuthRoute,
        props: { component: TruckDatabase },
    },
    {
        path: ApplicationPaths.Login,
        name: "Login",
        component: Login,
        props: { action: LoginActions.Login }
    },
    {
        path: ApplicationPaths.LoginFailed,
        name: "LoginFailed",
        component: Login,
        props: { action: LoginActions.LoginFailed }
    },
    {
        path: ApplicationPaths.LoginCallback,
        name: "LoginCallback",
        component: Login,
        props: { action: LoginActions.LoginCallback }
    },
    {
        path: ApplicationPaths.LogOut,
        name: "Logout",
        component: Logout,
        props: { action: LogoutActions.Logout }
    },
    {
        path: ApplicationPaths.LogOutCallback,
        name: "LogoutCallback",
        component: Logout,
        props: { action: LogoutActions.LogoutCallback }
    },
    {
        path: ApplicationPaths.LoggedOut,
        name: "LoggedOut",
        component: Logout,
        props: { action: LogoutActions.LoggedOut }
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;