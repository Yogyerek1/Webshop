import { Outlet } from 'react-router-dom'

const HomeLayout = () => {
    return (
        <div className="min-w-screen min-h-screen">
            <div></div>
            <main className="flex min-w-screen min-h-screen">
                <Outlet />
            </main>
            <div></div>
        </div>
    );
};

export default HomeLayout;