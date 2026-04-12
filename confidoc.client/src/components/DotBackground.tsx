import { useState, useEffect } from "react";

function DotBackground() {

  const [windowSize, setWindowSize] = useState({
      w: 0,
      h: 0,
  })
  
  useEffect(() => {
      function handleResize() {
          setWindowSize({
              w: window.innerWidth,
              h: window.innerHeight,
          })
      }
  
      handleResize();
  
      window.addEventListener('resize', handleResize);
  
      return () => window.removeEventListener('resize', handleResize);
  }, []);

  return (
      <div className="w-full flex flex-col-reverse absolute h-[550px] z-[-1]">
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 70))].map(
                      i => <div key={i} className="min-w-[50px] h-[50px] m-[9.2px] rounded-full bg-[var(--primary)]/1.75"></div>
                  )
              }
          </div>
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 65))].map(
                      i => <div key={i} className="min-w-[45px] h-[45px] m-[9.2px] rounded-full bg-[var(--primary)]/5"></div>
                  )
              }
          </div>
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 60))].map(
                      i => <div key={i} className="min-w-[40px] h-[40px] m-[9.3px] rounded-full bg-[var(--primary)]/10"></div>
                  )
              }
          </div>
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 55))].map(
                      i => <div key={i} className="min-w-[35px] h-[35px] m-[9.4px] rounded-full bg-[var(--primary)]/15"></div>
                  )
              }
          </div>
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 50))].map(
                      i => <div key={i} className="min-w-[30px] h-[30px] m-[9.5px] rounded-full bg-[var(--primary)]/20"></div>
                  )
              }
          </div>
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 45))].map(
                      i => <div key={i} className="min-w-[25px] h-[25px] m-[9.6px] rounded-full bg-[var(--primary)]/25"></div>
                  )
              }
          </div>
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 40))].map(
                      i => <div key={i} className="min-w-[20px] h-[20px] m-[9.7px] rounded-full bg-[var(--primary)]/30"></div>
                  )
              }
          </div>
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 35))].map(
                      i => <div key={i} className="min-w-[15px] h-[15px] m-[9.8px] rounded-full bg-[var(--primary)]/35"></div>
                  )
              }
          </div>
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 30))].map(
                      i => <div key={i} className="min-w-[10px] h-[10px] m-[9.9px] rounded-full bg-[var(--primary)]/40"></div>
                  )
              }
          </div>
          <div className="flex w-full justify-center mb-2">
              {
                  [...new Array(Math.floor(windowSize.w / 25))].map(
                      i => <div key={i} className="min-w-[5px] h-[5px] m-[10px] rounded-full bg-[var(--primary)]/45"></div>
                  )
              }
          </div>
      </div>
  );
}

export default DotBackground;