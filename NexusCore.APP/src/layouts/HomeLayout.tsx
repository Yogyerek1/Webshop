import { Outlet } from 'react-router-dom';
import { NavBar } from '../components/navigation/NavBar';
import { config } from '../config/config';

const HomeLayout = () => {
    return (
        <div className="flex flex-col min-w-screen min-h-screen">
            <NavBar props={config.navBar.props} />
            <main className="flex min-w-screen min-h-screen pt-25">
                <Outlet />
            </main>
            <div></div>
        </div>
    );
};

export default HomeLayout;